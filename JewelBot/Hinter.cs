using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace JewelBot
{

    class CoordinateActors
    {
        public Func<Size, int> dimensionSelector { get; set; }
        public Action<int> formExtentAssigner { get; set; }
        public Action<int> formStartAssigner { get; set; }
    }

    class Hinter
    {
        static HintForm[, ,] forms = new HintForm[8, 8, 2];
        public Hinter(BoardInfo board)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        forms[i, j, k] = createHintForm(board, i, j, k == 1);
                    }
        }

        private static HintForm createHintForm(BoardInfo board, int x, int y, bool vertical)
        {
            var hint = new HintForm();

            CoordinateActors xActors = new CoordinateActors
            {
                dimensionSelector = sz => sz.Width,
                formExtentAssigner = coord => hint.Width = coord,
                formStartAssigner = coord => hint.Left = coord
            };

            CoordinateActors yActors = new CoordinateActors
            {
                dimensionSelector = sz => sz.Height,
                formExtentAssigner = coord => hint.Height = coord,
                formStartAssigner = coord => hint.Top = coord
            };

            CoordinateActors len = vertical ? yActors : xActors;
            CoordinateActors wid = vertical ? xActors : yActors;

            var corner = board.getCellCorner(x, y);
            var cellSize = board.getCellSize(x, y);

            int hintWidth = 1;
            int hintLength = len.dimensionSelector(cellSize);
            var center = board.getCellCenter(x, y);

            len.formStartAssigner(len.dimensionSelector(new Size(center)) + hintLength / 3);
            wid.formStartAssigner(wid.dimensionSelector(new Size(center)) - hintWidth / 2);

            hint.StartPosition = FormStartPosition.Manual;

            hint.Width = 1;
            hint.Height = 1;

            hint.Show();

            len.formExtentAssigner(hintLength / 3);
            wid.formExtentAssigner(hintWidth);

            hint.Hide();

            return hint;
        }



        internal void show(int i, int j, bool vert, Color color)
        {
            forms[i, j, vert ? 1 : 0].BackColor = color;
            forms[i, j, vert ? 1 : 0].Show();
        }

        internal void hideAll()
        {
            foreach (var form in forms)
                if (form.Visible)
                    form.Hide();
        }
    }

}
