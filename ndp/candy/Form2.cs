//deniz toprak
//B211200026
//bla bla
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace candy
{
    public partial class Form2 : Form
    {
        private const int Rows = 8;
        private const int Columns = 8;
        private int remainingTime = 60; // Süre 60 saniye
        private int score = 0; 
        private bool isPaused = false; // Oyun durduruldu mu?
        private string playerName; 
        private Random random = new Random(); 
        private Button selectedButton = null; // Hareket için seçilen taşı saklar

        public Form2(string playerName)
        {
            InitializeComponent();
            this.playerName = playerName;
            this.KeyPreview = true; // Klavye olayları
            this.KeyDown += Form2_KeyDown; // Klavye olayı
        }

        private void Form2_Load(object sender, EventArgs e) // form yüklendiğinde
        {
            label4.Text = $"{playerName}";
            lblScore.Text = $"{score}";
            timer1.Interval = 1000; // saniye
            timer1.Start();// zamanlayıcı başlat
            lblTime.Text = $"{remainingTime} saniye";
            InitializeGameBoard();
        }

        private void timer1_Tick(object sender, EventArgs e) // zamanlayıcı
        {
            
            if (isPaused) return;

            remainingTime--;
            lblTime.Text = $"{remainingTime} saniye";

            if (remainingTime <= 0) 
            {
                timer1.Stop();
                SaveScore(playerName, score);
                MessageBox.Show($"Süre doldu! Toplam Puan: {score}", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P) // P tuşu ile dur
            {
                TogglePause();
            }
        }

        private void TogglePause()
        {
            // duraklatma durumunu değiştir
            isPaused = !isPaused;

            if (isPaused)
            {
                timer1.Stop();
                lblTime.Text = "Durduruldu";
            }
            else
            {
                timer1.Start();
                lblTime.Text = $"{remainingTime} saniye";
            }
        }

        private void InitializeGameBoard() // tahtayı oluştur
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = Rows;
            tableLayoutPanel1.ColumnCount = Columns;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();

            for (int i = 0; i < Rows; i++)
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / Rows));
            for (int j = 0; j < Columns; j++)
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / Columns));

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    AddTileToGameBoard(i, j);
                }
            }

            // Tahtayı temizle ama skoru artırma
            while (CheckAndRemoveMatches(false)) // Skor artırma kapalı
            {
                DoldurBoşluklar();
            }

            score = 0; // Skoru sıfırla
            lblScore.Text = $"{score}";
        }


        private void AddTileToGameBoard(int row, int col)
        {
            Button tileButton = CreateRandomTile();
            tableLayoutPanel1.Controls.Add(tileButton, col, row); //rastgele taş ata
        }

        private Button CreateRandomTile() // rastgele taş oluştur
        {
            string[] normalTiles = { "Blue", "Green", "Red", "Yellow" };
            string[] jokerTiles = { "Roket_H", "Roket_V", "Kopter", "Bomb", "Rainbow" };

            ITile selectedTile;

            if (random.Next(100) < 98) // normal taş oranını arttır
            {
                string type = normalTiles[random.Next(normalTiles.Length)];
                selectedTile = new Tile(type, $"Resources/{type.ToLower()}_gem.jpg");
            }
            else
            {
                string type = jokerTiles[random.Next(jokerTiles.Length)];
                selectedTile = new Tile(type, $"Resources/{type.ToLower()}.jpg");
            }

            Button tileButton = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(1),
                Tag = selectedTile,
                BackgroundImage = Image.FromFile(selectedTile.ImagePath),
                BackgroundImageLayout = ImageLayout.Stretch
            };
            tileButton.Click += TileButton_Click;
            return tileButton;
        }

        private void TileButton_Click(object sender, EventArgs e) // taş seçme taş swap
        {
            if (isPaused) return;

            Button clickedButton = sender as Button;
            Tile tile = clickedButton.Tag as Tile;

            if (tile.Type.StartsWith("Roket") || tile.Type == "Kopter" || tile.Type == "Bomb" || tile.Type == "Rainbow") //jokerse
            {
                PerformJokerAction(tile, clickedButton);
                CheckAndRemoveMatches();
                DoldurBoşluklar();
                return;
            }

            if (selectedButton == null)
            {
                selectedButton = clickedButton;
                clickedButton.BackColor = Color.LightGray;
            }
            else
            {
                var position1 = tableLayoutPanel1.GetPositionFromControl(selectedButton);
                var position2 = tableLayoutPanel1.GetPositionFromControl(clickedButton);

                if (Math.Abs(position1.Row - position2.Row) + Math.Abs(position1.Column - position2.Column) == 1) //komşuları hespla
                {
                    SwapTiles(selectedButton, clickedButton);

                    if (!CheckAndRemoveMatches()) //eşleşme yoksa dön
                    {
                        SwapTiles(selectedButton, clickedButton);
                    }
                    else
                    {
                        DoldurBoşluklar(); //patlat
                    }
                }

                selectedButton.BackColor = Color.Transparent;
                selectedButton = null;
            }
        }

        private void SwapTiles(Button button1, Button button2) // seçilen ile tıklanan değiştir
        {
            Tile tempTile = button1.Tag as Tile;
            button1.Tag = button2.Tag;
            button1.BackgroundImage = button2.BackgroundImage;
            button2.Tag = tempTile;
            button2.BackgroundImage = Image.FromFile(tempTile.ImagePath);
        }

        private void PerformJokerAction(Tile tile, Button clickedButton) //joker özellikler
        {
            int removedTiles = 0; // Kaldırılan taş sayısını takip etmek için değişken

            switch (tile.Type)
            {
                case "Roket_H":
                    removedTiles = PatlatYatay(clickedButton);
                    break;
                case "Roket_V":
                    removedTiles = PatlatDikey(clickedButton);
                    break;
                case "Kopter":
                    removedTiles = PatlatRastgele();
                    break;
                case "Bomb":
                    removedTiles = PatlatCevre(clickedButton);
                    break;
                case "Rainbow":
                    removedTiles = PatlatAyniRenk(clickedButton);
                    break;
            }

            // Kaldırılan taş sayısına göre puanı artır
            score += removedTiles * 5;
            lblScore.Text = $"{score}"; // Güncel skoru göster

            // Joker taşı kaldır
            tableLayoutPanel1.Controls.Remove(clickedButton);
        }

        private int PatlatYatay(Button clickedButton)
        {
            var position = tableLayoutPanel1.GetPositionFromControl(clickedButton);
            int row = position.Row;
            int removedTiles = 0;

            for (int col = 0; col < Columns; col++)
            {
                if (GetTileAt(row, col) != null)
                {
                    RemoveTileAt(row, col);
                    removedTiles++;
                }
            }

            return removedTiles;
        }

        private int PatlatDikey(Button clickedButton)
        {
            var position = tableLayoutPanel1.GetPositionFromControl(clickedButton);
            int col = position.Column;
            int removedTiles = 0;

            for (int row = 0; row < Rows; row++)
            {
                if (GetTileAt(row, col) != null)
                {
                    RemoveTileAt(row, col);
                    removedTiles++;
                }
            }

            return removedTiles;
        }

        private int PatlatRastgele()
        {
            int randomRow = random.Next(Rows);
            int randomCol = random.Next(Columns);
            int removedTiles = 0;

            if (GetTileAt(randomRow, randomCol) != null)
            {
                RemoveTileAt(randomRow, randomCol);
                removedTiles++;
            }

            return removedTiles;
        }

        private int PatlatCevre(Button clickedButton)
        {
            var position = tableLayoutPanel1.GetPositionFromControl(clickedButton);
            int row = position.Row;
            int col = position.Column;
            int removedTiles = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                    {
                        if (GetTileAt(newRow, newCol) != null)
                        {
                            RemoveTileAt(newRow, newCol);
                            removedTiles++;
                        }
                    }
                }
            }

            return removedTiles;
        }

        private int PatlatAyniRenk(Button clickedButton)
        {
            var position = tableLayoutPanel1.GetPositionFromControl(clickedButton);
            var targetTile = GetTileAt(position.Row, position.Column);
            if (targetTile == null) return 0;

            string targetType = targetTile.Type;
            int removedTiles = 0;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var tile = GetTileAt(row, col);
                    if (tile != null && tile.Type == targetType)
                    {
                        RemoveTileAt(row, col);
                        removedTiles++;
                    }
                }
            }

            return removedTiles;
        }

        // dinamik kontrol ve eşleşen taş*5 puan
        private bool CheckAndRemoveMatches(bool increaseScore = true) // Skor artırma kontrolü için parametre
        {
            List<(int row, int col)> tilesToRemove = new List<(int row, int col)>();
            bool matchFound = false;

            // Yatay eşleşmeleri kontrol et
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns - 2; col++)
                {
                    var tile1 = GetTileAt(row, col);
                    var tile2 = GetTileAt(row, col + 1);
                    var tile3 = GetTileAt(row, col + 2);

                    if (tile1 != null && tile2 != null && tile3 != null &&
                        tile1.Type == tile2.Type && tile2.Type == tile3.Type)
                    {
                        tilesToRemove.Add((row, col));
                        tilesToRemove.Add((row, col + 1));
                        tilesToRemove.Add((row, col + 2));
                        matchFound = true;
                    }
                }
            }

            // Dikey eşleşmeleri kontrol et
            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows - 2; row++)
                {
                    var tile1 = GetTileAt(row, col);
                    var tile2 = GetTileAt(row + 1, col);
                    var tile3 = GetTileAt(row + 2, col);

                    if (tile1 != null && tile2 != null && tile3 != null &&
                        tile1.Type == tile2.Type && tile2.Type == tile3.Type)
                    {
                        tilesToRemove.Add((row, col));
                        tilesToRemove.Add((row + 1, col));
                        tilesToRemove.Add((row + 2, col));
                        matchFound = true;
                    }
                }
            }

            // Taşları kaldır
            foreach (var (row, col) in tilesToRemove.Distinct())
            {
                RemoveTileAt(row, col);
            }

            // Skoru artırma kontrolü
            if (increaseScore)
            {
                score += tilesToRemove.Count * 5;
                lblScore.Text = $"{score}";
            }

            return matchFound;
        }


        // boş kalan hücreleri doldur
        private void DoldurBoşluklar()
        {
            for (int col = 0; col < Columns; col++)
            {
                for (int row = Rows - 1; row >= 0; row--)
                {
                    var control = tableLayoutPanel1.GetControlFromPosition(col, row);
                    if (control == null)
                    {
                        for (int k = row - 1; k >= 0; k--)
                        {
                            var aboveControl = tableLayoutPanel1.GetControlFromPosition(col, k);
                            if (aboveControl != null)
                            {
                                tableLayoutPanel1.SetCellPosition(aboveControl, new TableLayoutPanelCellPosition(col, row));
                                break;
                            }
                        }

                        if (tableLayoutPanel1.GetControlFromPosition(col, row) == null)
                        {
                            Button newTile = CreateRandomTile();
                            tableLayoutPanel1.Controls.Add(newTile, col, row);
                        }
                    }
                }
            }

            if (CheckAndRemoveMatches())
            {
                DoldurBoşluklar();
            }
        }

        private Tile GetTileAt(int row, int col)
        {
            var control = tableLayoutPanel1.GetControlFromPosition(col, row);
            return control?.Tag as Tile;
        }

        private void RemoveTileAt(int row, int col)
        {
            var control = tableLayoutPanel1.GetControlFromPosition(col, row);
            if (control != null)
            {
                tableLayoutPanel1.Controls.Remove(control);
            }
        }

        //skorları kaydet
        private void SaveScore(string playerName, int playerScore)
        {
            string filePath = "skor.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"{playerName}:{playerScore}");
            }
        }
    }

    //get-set kullanımı
    //public class Tile : ITile
    //{
    //    public string Type { get; set; }
    //    public string ImagePath { get; set; }

    //    public Tile(string type, string imagePath)
    //    {
    //        Type = type;
    //        ImagePath = imagePath;
    //    }
    //}
}
