using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JewelBot
{
    static class StateUtils
    {
        public static Coin get(this Coin[,] state, Cell cell)
        {
            return state[cell.I, cell.J];
        }
        public static Coin getSafe(this Coin[,] state, Cell cell)
        {
            if (cell.isValid())
                return state.get(cell);
            else
                return Coin.None;
        }
        public static void put(this Coin[,] state, Cell cell, Coin coin)
        {
            state[cell.I, cell.J] = coin;
        }

        public static bool isValid(this Cell cell)
        {
            return cell.I >= 0 && cell.I < 8 && cell.J >= 0 && cell.J < 8;
        }


        private static char getCoinChar(Coin coin)
        {
            switch (coin)
            {
                case Coin.Black: return 'k';
                case Coin.Blue: return 'b';
                case Coin.Green: return 'g';
                case Coin.Purple: return 'p';
                case Coin.Red: return 'r';
                case Coin.Yellow: return 'y';
                default: return '-';
            }
        }

        public static void Print(this Coin[,] state)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(getCoinChar(state[i, j]));
                }
                Console.WriteLine();
            }
        }
    }
}
