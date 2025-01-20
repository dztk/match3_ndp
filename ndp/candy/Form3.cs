// DENİZ TOPRAK
// B211200026
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace candy
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            LoadHighScores();
        }

        private void LoadHighScores()
        {
            string filePath = "skor.txt"; // Skor dosyasının yolu

            // dosya yoksa
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Henüz skor kaydı bulunmamaktadır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Dosyadan skorları oku ve sırala
            List<string> scores = File.ReadAllLines(filePath)
                                      .OrderByDescending(line => int.Parse(line.Split(':')[1]))
                                      .Take(5)
                                      .ToList();

            // Skorları ListBox içine ekle
            int index = 1; // Başlangıç numarası
            foreach (var score in scores)
            {
                listBox1.Items.Add($"{index}) {score.Split(':')[0]}: {score.Split(':')[1]} puan");
                index++; // sıra no arttır
            }
        }

        
    }
}
