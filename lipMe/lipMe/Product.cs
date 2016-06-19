using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace lipMe
{
    public class Product
    {
        public enum ProductTypeEnum { Lipstick, Eyeshadow, NailPolish}

        public string Name { get; set; }
        public SolidColorBrush Color { get; set; }
        public string ImageUrl { get; set; }
        public string ProductUrl { get; set; }
        public string Company { get; set; }
        public string Price { get; set; }
        public ProductTypeEnum Type { get; set; }


        public double GetColorDistance( Product other )
        {
            double distR = Color.Color.R - other.Color.Color.R;
            double distG = Color.Color.G - other.Color.Color.G;
            double distB = Color.Color.B - other.Color.Color.B;
            return Math.Sqrt( distR*distR + distG*distG + distB*distB );
        }
    }
}
