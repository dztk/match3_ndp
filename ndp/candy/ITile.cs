
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace candy
{
    public interface ITile
    {
        string Type { get; set; }
        string ImagePath { get; set; }
    }
}
