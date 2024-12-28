using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BagelAura
{
    public partial class CPUDisplay : Form
    {
        public Color activecolor = Color.Black;

        public CPUDisplay()
        {
            InitializeComponent();

            this.Bounds = new Rectangle(0, 0, 1100, 100);
            this.TopMost = true;
            Point TopLeft = Screen.AllScreens[0].WorkingArea.Location;
            TopLeft.Y = TopLeft.Y + 2400;
            this.Location = TopLeft;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.Visible = true;
        }

        private void CPUDisplay_Paint(object sender, PaintEventArgs e)
        {
            using (var gfx = e.Graphics) {
                using (var brush = new SolidBrush(this.activecolor))
                {
                    gfx.FillRectangle(brush, 0, 0, 1100, 100);
                }
            }
        }
    }
}
