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
        private int graphWidth = 1100;
        private int graphHeight = 150;
        private float graphLine = 15.0F;

        public Color currentcolor = Color.Black;
        public int currentload = 0;

        public Point[] graphPoints = new Point[10];
        public Color[] graphColors = new Color[10];

        private int segmentWidth;

        public CPUDisplay()
        {
            InitializeComponent();

            this.Bounds = new Rectangle(0, 0, graphWidth, graphHeight);
            this.TopMost = true;
            Point TopLeft = Screen.AllScreens[0].WorkingArea.Location;
            TopLeft.Y = TopLeft.Y + 2400;
            this.Location = TopLeft;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.Visible = true;

            segmentWidth = graphWidth / graphPoints.Length;
        }

        private void CPUDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (currentload < 0) currentload = 0;
            if (currentload > 100) currentload = 100;

            int i = 0;
            for(; i < graphPoints.Length - 1;i++)
            {
                graphPoints[i].X = segmentWidth * i;
                graphPoints[i].Y = graphPoints[i+1].Y;
            }
            graphPoints[i].X = segmentWidth * i;
            graphPoints[i].Y = graphHeight - (int) graphLine - currentload;

            using (var gfx = e.Graphics) {
                using (var graphPen = new Pen(new SolidBrush(this.currentcolor)))
                {
                    graphPen.Width = graphLine;
                    graphPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    gfx.DrawLines(graphPen, graphPoints);
                }
            }
        }
    }
}
