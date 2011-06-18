using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace JewelBot
{
    class HintForm : Form
    {
        static HintForm()
        {
        }
        public HintForm()
        {
            this.TopMost = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.White;
            this.AutoSize = false;
            this.WindowState = FormWindowState.Normal;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ShowInTaskbar = false;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.Hide();
        }
    }
}
