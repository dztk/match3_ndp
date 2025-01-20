using System;
using System.Windows.Forms;

namespace candy
{
    static class Program
    {
        /// <summary>
        /// Programın ana giriş noktasıdır.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Başlangıç formu
        }
    }
}
