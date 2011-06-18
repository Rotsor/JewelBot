using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JewelBot
{
    class Cell
    {
        public static readonly Cell Zero = new Cell(0,0);
        public Cell(int i, int j)
        {
            this.I = i;
            this.J = j;
        }
        public int I { get; set; }
        public int J { get; set; }
        public override string ToString()
        {
            return "(" + I + ", " + J + ")";
        }
    }

    class Offset
    {
        public Offset(int di, int dj)
        {
            this.Di = di;
            this.Dj = dj;
        }
        public int Di { get; set; }
        public int Dj { get; set; }

        public static readonly Offset Zero = new Offset(0, 0);
        public static readonly Offset Vertical = new Offset(1, 0);
        public static readonly Offset Horisontal = new Offset(0, 1);
    }

}
