using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JewelBot
{
    class Move
    {
        public Cell Cell { get; set; }
        public bool Vertical { get; set; }
        public override string ToString()
        {
            return (Vertical ? "vert" : "horiz") + " from " + Cell;
        }
    }

    static class MoveUtils
    {
        public static Cell DestinationCell(this Move move)
        {
            return Plus(move.Cell, move.Vertical ? Offset.Vertical : Offset.Horisontal);
        }

        public static Offset Minus(this Cell cell2, Cell cell1)
        {
            return new Offset(cell2.I - cell1.I, cell2.J - cell1.J);
        }

        public static Cell Plus(this Cell cell, Offset offset)
        {
            return new Cell(cell.I + offset.Di, cell.J + offset.Dj);
        }

        public static Offset Plus(this Offset offset1, Offset offset2)
        {
            return new Offset(offset1.Di + offset2.Di, offset1.Dj + offset2.Dj);
        }

        public static Offset Multiply(this Offset offset, int multiplier)
        {
            return new Offset(offset.Di * multiplier, offset.Dj * multiplier);
        }

        public static Offset RotateCW(this Offset offset)
        {
            return new Offset(offset.Dj, -offset.Di);
        }

        public static Offset RotateCCW(this Offset offset)
        {
            return new Offset(-offset.Dj, offset.Di);
        }
    }
}
