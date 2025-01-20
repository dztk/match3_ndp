// deniz toprak
//b211200026
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace candy
{
    public class Tile : ITile // interface ile tile sınıfından olan taşların type ve imagepath zorunlu hale getir
    {
        public string Type { get; set; }
        public string ImagePath { get; set; }

        public Tile(string type, string imagePath)
        {
            Type = type;
            ImagePath = imagePath;
        }
    }
}
