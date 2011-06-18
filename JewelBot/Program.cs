using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;

namespace JewelBot
{
    enum Coin
    {
        None=0,
        Black,
        Red,
        Yellow,
        Blue,
        Green,
        Purple
    }
    class BoardInfo
    {
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }
    }

    class Program
    {
        static Dictionary<Coin,Color> coinColors = new Dictionary<Coin,Color> { 
            {Coin.Red, Color.FromArgb(0x9c, 0x28, 0x0e)}, // red +
            {Coin.Black, Color.FromArgb(0x9e, 0x9d, 0x9d)}, // black +
            {Coin.Yellow, Color.FromArgb(0x5b, 0xb3, 0x81)}, // yellow +
            {Coin.Blue, Color.FromArgb(0x4b, 0xa4, 0xc7)}, // blue +
            {Coin.Green, Color.FromArgb(0x62, 0x9f, 0x47)}, // green +
            {Coin.Purple, Color.FromArgb(0x83, 0x38, 0x8d)} // purple +
        };

        static void Main(string[] args)
        {
            var dir = ".";
            if (args.Length > 0)
                dir = args[0];
            var scale = 2;
            if (args.Length > 1)
                scale = int.Parse(args[1]);
            var sleep = 30000;
            if (args.Length > 2)
                sleep = int.Parse(args[2])*1000;
            while (true)
            {
                makeScreenshot(dir, scale);
                mySleep(sleep);
            }
        }

        static void runStarJewelledBot(string[] args)
        {

                        

/*            while (true)
            {
                Thread.Sleep(10);
                WinApi.doWithDesktopDc(dc =>
                {
                    using (var gr = Graphics.FromHdc(dc))
                        gr.FillRectangle(new SolidBrush(Color.Black), 0, 0, 1000, 800);
                });
                Thread.Sleep(10);
                Console.WriteLine("qwe");
            }*/

           // Thread.Sleep(1000);
           // var form = new HintForm();
           // form.Show();
            //form.TopLevel = true;
           // while (true) { Application.DoEvents(); form.BringToFront(); }


            //var board = readBoardInfo();
            var board = new BoardInfo { BottomRight = new Point { X = 1882, Y = 677 }, TopLeft = new Point { X = 1306, Y = 101 } };

            var hinter = new Hinter(board);

            while(true) {
                var colorsMap = time(() => readMap(board));
                var coinsMap = parseMap(colorsMap);
                //printColorsMap(colorsMap);
                //printMap(coinsMap);
                var moves = time(() => Solver.GetBestMoveSequence(coinsMap), "solved");
                hinter.hideAll();
                showHints(hinter, moves);
                mySleep(300);
            }
        }

        private static void printColorsMap(Color[,] colorsMap)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var c = colorsMap[i, j];
                    Console.Write("{0:x02}{1:x02}{2:x02} ", c.R, c.G, c.B);
                }
                Console.WriteLine();
            }
        }

        private static void printMap(Coin[,] map)
        {
            map.Print();
        }

        private static void showHints(Hinter hinter, IEnumerable<Move> moves)
        {
            Color[] colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Lime, Color.Green, Color.Cyan, Color.LightBlue, Color.Blue };
            int cnt = 0;
            foreach (var move in moves)
            {
                hinter.show(move.Cell.I, move.Cell.J, move.Vertical, colors[cnt++]);
                //break;
            }
            /*for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    foreach(var vert in new[]{true, false})
                    {
                        if (isGoodMove(map, i, j, vert))
                            hinter.show(i, j, vert);
                    }*/
        }


        private static bool isGoodMove(Coin[,] map, int i, int j, bool vert)
        {
            int newi = i + (vert ? 1 : 0);
            int newj = j + (vert ? 0 : 1);
            if(newi>=8 || newj>=8) return false;

            Coin[,] newMap = (Coin[,])map.Clone();

            newMap[newi, newj] = map[i, j];
            newMap[i, j] = map[newi, newj];

            return isDestroyed(newMap, i, j) || isDestroyed(newMap, newi, newj);
        }

        private static bool isDestroyed(Coin[,] newMap, int newi, int newj)
        {
            for (int ti = 0; ti < 8; ti++)
                for (int tj = 0; tj < 8 - 2; tj++)
                {
                    bool pointIncluded = false;
                    for (int k = 0; k < 3; k++)
                    {
                        if (ti == newi && tj + k == newj)
                            pointIncluded = true;
                        if (newMap[ti, tj + k] != newMap[ti, tj]) goto bad;
                    }
                    if (pointIncluded)
                        return true;
                bad: ;
                }
            for (int ti = 0; ti < 8-2; ti++)
                for (int tj = 0; tj < 8; tj++)
                {
                    bool pointIncluded = false;
                    for (int k = 0; k < 3; k++)
                    {
                        if (ti + k == newi && tj == newj)
                            pointIncluded = true;
                        if (newMap[ti + k, tj] != newMap[ti, tj]) goto bad;
                    }
                    if (pointIncluded)
                        return true;
                bad: ;
                }
            return false;
        }

        private static BoardInfo readBoardInfo()
        {
            var cornersDelay = 5000;
            Console.WriteLine("Point to the upper-left corner please");
            Thread.Sleep(cornersDelay);
            var top_left = Cursor.Position;

            Console.WriteLine("Point to the lower-right corner please");
            Thread.Sleep(cornersDelay);
            var bottom_right = Cursor.Position;

            var board = new BoardInfo { BottomRight = bottom_right, TopLeft = top_left };

            Console.WriteLine("board: " + board.BottomRight + "; " + board.TopLeft);
            return board;
        }

        private static void mySleep(int timeMilliseconds)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < timeMilliseconds)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }

        private static Coin[,] parseMap(Color[,] color)
        {
            Coin[] coinValues = new Coin[] { Coin.Red, Coin.Black, Coin.Blue, Coin.Green, Coin.Purple, Coin.Yellow };
            Coin[,] result = new Coin[8, 8];
            for(int i=0;i<8;i++)
                for(int j=0;j<8;j++)
                    result[i, j] = coinValues.OrderBy(coin => diff(coin, color[i, j])).First();
            return result;
        }

        private static int diff(Coin coin, Color color)
        {
            return diff(coinColors[coin], color);
        }

        public static T time<T>(Func<T> func)
        {
            return time(func, "unnamed action");
        }

        public static T time<T>(Func<T> func, String comment)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                return func();
            }
            finally
            {
                sw.Stop();
                Console.WriteLine(comment + ": " + sw.Elapsed);
            }
        }

        private static int diff(Color color_1, Color color_2)
        {
            var a1 = new[] { color_1.R, color_1.B, color_1.G }.Min();
            var a2 = new[] { color_2.R, color_2.B, color_2.G }.Min();
            return
                Math.Abs(color_1.R-a1 - color_2.R+a2) +
                Math.Abs(color_1.G-a1 - color_2.G+a2) +
                Math.Abs(color_1.B-a1 - color_2.B+a2);
        }

        private static void makeScreenshot(String directory, int scaleFactor)
        {
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var scaledWidth = screenBounds.Width / scaleFactor;
            var scaledHeight = screenBounds.Height / scaleFactor;
            using (Bitmap bmp = new Bitmap(screenBounds.Width, screenBounds.Height))
            {
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(Point.Empty, Point.Empty, bmp.Size);
                }


                Console.WriteLine("making thumbnail...");
                var thumb = bmp.GetThumbnailImage(scaledWidth, scaledHeight, () => { return false; }, IntPtr.Zero);
                var timeString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                thumb.Save(Path.Combine(directory, timeString+".jpg"), ImageFormat.Jpeg);
            }
        }

        private static Color[,] readMap(BoardInfo board)
        {
            Color[,] result = new Color[8, 8];
            var boardSize = board.BottomRight.Minus(board.TopLeft);
            using (Bitmap bmp = new Bitmap(boardSize.Width, boardSize.Height))
            {
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(board.TopLeft, Point.Empty, bmp.Size);
                }

                
                Console.WriteLine("making thumbnail...");
                var thumb = bmp.GetThumbnailImage(8*3, 8*3, () => { return false; }, IntPtr.Zero);

                //showBitmap(bmp);

                for (int i = 0; i < 8; i++)
                {

                    for (int j = 0; j < 8; j++)
                    {
                        Console.Write("*");
                        Point lu = board.getCellCorner(i, j);
                        Point rd = board.getCellCorner(i + 1, j + 1);
                        List<Color> pixels = new List<Color>();

                        pixels.Add(((Bitmap)thumb).GetPixel(j*3+1, i*3+1));
                       /* for (int y = lu.Y; y < rd.Y; y++)
                            for (int x = lu.X; x < rd.X; x++)
                                pixels.Add(bmp.GetPixel(x - top_left.X, y - top_left.Y));*/

                        int r = (int)pixels.Average(c => c.R);
                        int g = (int)pixels.Average(c => c.G);
                        int b = (int)pixels.Average(c => c.B);
                        result[i, j] = Color.FromArgb(r, g, b);
                    }
                }
            }

            Console.WriteLine("");

            return result;
        }

        private static void showBitmap(Bitmap bmp)
        {
            var form = new Form();
            form.Controls.Add(new PictureBox() { Image = bmp, Size = bmp.Size });
            form.Show();
            Application.Run(form);
        }
    }
}
