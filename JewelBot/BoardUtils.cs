using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace JewelBot
{
    static class BoardUtils
    {
        public static Point getCellCenter(this BoardInfo board, int x, int y)
        {
            var p = board.getCellCorner(x, y);
            return Point.Add(p, getCellSize(board, x, y).Multiply(0.5));
        }
        public static Size getCellSize(this BoardInfo board, int x, int y)
        {
            return getCellCorner(board, x + 1, y + 1).Minus(getCellCorner(board, x, y));
        }

        public static Point getCellCorner(this BoardInfo board, int i, int j)
        {
            var bottomRight = board.BottomRight;
            var topLeft = board.TopLeft;
            Size size = new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
            return new Point(topLeft.X + (size.Width * j / 8), topLeft.Y + (size.Height * i / 8));
        }
    }
}
