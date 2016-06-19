using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lipMe
{
    public class PixelColor
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public double GetColorDistance(Product other)
        {
            double distR = Red - other.Color.Color.R;
            double distG = Green - other.Color.Color.G;
            double distB = Blue - other.Color.Color.B;
            return Math.Sqrt(distR * distR + distG * distG + distB * distB);
        }
    }
}
