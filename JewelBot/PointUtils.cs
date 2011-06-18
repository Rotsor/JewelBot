using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace JewelBot
{
    static class PointUtils
    {
        public static Size Minus(this Point p1, Point p2) 
        {
            return Size.Subtract(new Size(p1), new Size(p2));
        }

        public static Size Multiply(this Size size, double p)
        {
            return new Size((int)(size.Width * p), (int)(size.Height * p));
        }
    }
}
