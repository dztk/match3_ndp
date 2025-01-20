
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using candy;
using System.IO;  


namespace candy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // TextBox'tan oyuncu adını al
            string playerName = textBox1.Text;

            // Oyuncu adı boşsa uyar
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Lütfen bir oyuncu adı girin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Hide();  // Giriş ekranını gizler 
                          

            Form2 form2 = new Form2(playerName);
            form2.Show();  // MainForm'u yeni pencerede açar
            //this.Close();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog(); // Yeni formu modal olarak açar
            // formu kapatmadan oyunda bir şey yapamazsın
        }
    }
}
