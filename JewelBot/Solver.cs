using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JewelBot
{
    class Solver
    {

        public static void PrintPlan(Coin[,] state, IEnumerable<Move> plan)
        {
            int cnt = 0;
            foreach (var move in plan)
            {
                Console.WriteLine("step " + cnt);
                state.Print();
                Console.WriteLine("making move: " + move);
                var moveRes = MakeMove(state, move);
                Console.WriteLine("destroyed " + moveRes.Item2);
                state = moveRes.Item1;
            }
        }

        public static IEnumerable<Move> GetBestMoveSequence(Coin[,] state)
        {
            var res = GetBestMoveRec(state, 3);
            PrintPlan(state, res.Item1);
            return res.Item1;
        }

        private static Tuple<Coin[,], int> MakeMove(Coin[,] state, Move move)
        {
            return destroy(exchange(state, move));
        }

        private static Tuple<Coin[,], int> destroy(Coin[,] coin)
        {
            int score = 0;
            Coin[,] state = coin;
            int scoreIncrement;
            int multiplier = 1;
            do
            {
                var res = destroyOnce(state);
                state = res.Item1;
                scoreIncrement = res.Item2;
                score += scoreIncrement * multiplier;
                multiplier++;
            } while (scoreIncrement > 0);
            return Tuple.Create(state, score);
        }

        private static Tuple<Coin[,], int> destroyOnce(Coin[,] state)
        {
            Coin[,] result = Clone(state);
            int score = 0;

            Func<Cell, int, Offset, Offset, int> destroySingleCluster = (start, count, drow, dcol) =>
            {
                int totalCount = count;
                Coin color = result.get(start);
                if (color == Coin.None)
                    return 0;
                for(int i=0;i<count;i++)
                {
                    Cell cell = start.Plus(dcol.Multiply(i));
                    int neighboursP = 0;
                    int neighboursN = 0;
                    for (int j = 1; j < 8; j++)
                    {
                        if (result.getSafe(cell.Plus(drow.Multiply(j))) != color)
                            break;
                        neighboursP++;
                    }
                    for (int j = 1; j < 8; j++)
                    {
                        if (result.getSafe(cell.Plus(drow.Multiply(-j))) != color)
                            break;
                        neighboursN++;
                    }
                    var neighbours = neighboursP + neighboursN;
                    if (neighbours >= 2)
                    {
                        totalCount += neighbours;
                        for (int j = 1; j < neighboursP; j++)
                            result.put(cell.Plus(drow.Multiply(j)), Coin.None);
                        for (int j = 1; j < neighboursN; j++)
                            result.put(cell.Plus(drow.Multiply(-j)), Coin.None);
                    }
                    result.put(cell, Coin.None);
                }
                return totalCount - 2;
            };

            Action<Offset, Offset> gogo = (drow, dcol) =>
            {
                Cell rowStart = Cell.Zero;
                for (int i = 0; i < 8; i++)
                {
                    Coin lastCoin = Coin.None;
                    int similarCoins = 0;
                    Cell cur = rowStart;
                    for (int j = 0; j < 9; j++)
                    {
                        if (lastCoin == result.getSafe(cur))
                            similarCoins++;
                        else
                        {
                            if(lastCoin != Coin.None && similarCoins >= 3)
                                score += destroySingleCluster(cur.Plus(dcol.Multiply(-similarCoins)), similarCoins, drow, dcol);
                            similarCoins = 1;
                            lastCoin = result.getSafe(cur);
                        }

                        cur = cur.Plus(dcol);
                    }
                    rowStart = rowStart.Plus(drow);
                }
            };

            gogo(Offset.Vertical, Offset.Horisontal);
            gogo(Offset.Horisontal, Offset.Vertical);
            
            return Tuple.Create(drop(result), score);
        }

        private static Coin[,] drop(Coin[,] state)
        {
            Coin[,] result = new Coin[8,8];
            for(int j=0;j<8;j++)
            {
                int k = 7;
                for (int i = 7; i >= 0; i--)
                {
                    if (state[i, j] != Coin.None)
                    {
                        result[k--, j] = state[i, j];
                    }
                }
            }
            return result;
        }

        private static Coin[,] Clone(Coin[,] state)
        {
            return (Coin[,])state.Clone();
        }

        private static Coin[,] exchange(Coin[,] state, Move move)
        {
            Coin[,] result = Clone(state);
            result.put(move.DestinationCell(), state.get(move.Cell));
            result.put(move.Cell, state.get(move.DestinationCell()));
            return result;
        }

        private static Tuple<IEnumerable<Move>,int> GetBestMoveRec(Coin[,] state, int depth)
        {
            var emptyResult = Tuple.Create<IEnumerable<Move>, int>(new Move[] { }, 0);
            if(depth == 0)
                return emptyResult;
            Move[] availableMoves = GetAllMoves(state);
            return availableMoves.Select(move =>
            {
                var moveRes = MakeMove(state, move);
                var rec = GetBestMoveRec(moveRes.Item1, depth - 1);
                IEnumerable<Move> moves = rec.Item1;
                return Tuple.Create(new[]{move}.Concat(moves), moveRes.Item2 + (rec.Item2*2/3));
            }).OrderByDescending(t => t.Item2).FirstOrDefault() ?? emptyResult;
        }

        private static Move[] GetAllMoves(Coin[,] state)
        {
            return GetAllMoves().Where(m => isValidMove(state, m)).ToArray();
        }

        private static bool isValidMove(Coin[,] state, Move m)
        {
            return 
                isUsefulMove(state, m.Cell, m.DestinationCell()) ||
                isUsefulMove(state, m.DestinationCell(), m.Cell)
                ;
        }

        private static bool isUsefulMove(Coin[,] state, Cell source, Cell dest)
        {
            Offset delta = dest.Minus(source);
            var right = delta.RotateCW();
            var left = delta.RotateCCW();
            var color = state.get(source);
            if(color == Coin.None)
                return false;
            if (state.getSafe(dest.Plus(delta)) == color && state.getSafe(dest.Plus(delta).Plus(delta)) == color)
                return true;
            if (state.getSafe(dest.Plus(right)) == color && state.getSafe(dest.Plus(right).Plus(right)) == color)
                return true;
            if (state.getSafe(dest.Plus(left)) == color && state.getSafe(dest.Plus(left).Plus(left)) == color)
                return true;
            if (state.getSafe(dest.Plus(left)) == color && state.getSafe(dest.Plus(right)) == color)
                return true;
            return false;
        }

        private static Move[] GetAllMoves()
        {
            List<Move> result = new List<Move>();
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 8; j++)
                    result.Add(new Move { Cell = new Cell(i, j), Vertical = true });
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 7; j++)
                    result.Add(new Move { Cell = new Cell(i, j), Vertical = false });
            return result.ToArray();
        }
    }
}
