using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltUI.Forms;

namespace BagelAura
{
    public partial class CPUDisplay : DarkForm
    {
        private int graphWidth = 713;
        private int graphHeight = 140;
        private float graphLine = 15.0F;

        public Color currentcolor = Color.Black;
        public int currentload = 0;

        public Point[] graphPoints = new Point[71];
        public Color[] graphColors = new Color[71];

        private int segmentWidth;

        public CPUDisplay()
        {
            InitializeComponent();

            this.Bounds = new Rectangle(10, 0, graphWidth, graphHeight);
            this.TopMost = true;
            Point TopLeft = System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Location;
            TopLeft.X = TopLeft.X + 10;
            TopLeft.Y = TopLeft.Y + 2350;
            this.Location = TopLeft;
            //this.Bounds = new Rectangle(TopLeft.X, TopLeft.Y, graphWidth, graphHeight);

            segmentWidth = graphWidth / graphPoints.Length;
        }

        private void CPUDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (currentload < 0) currentload = 0;
            if (currentload > 100) currentload = 100;

            using (var gfx = e.Graphics)
            {
                int i = 0;
                for (; i < graphPoints.Length - 1; i++)
                {
                    graphPoints[i].X = segmentWidth * i;

                    Point[] points = {  new Point(graphPoints[i].X, graphPoints[i].Y), 
                                        new Point(graphPoints[i + 1].X, graphPoints[i + 1].Y), 
                                        new Point(graphPoints[i + 1].X, graphHeight),
                                        new Point(graphPoints[i].X, graphHeight) };

                    using (var brush = new SolidBrush(graphColors[i]))
                    {
                        gfx.FillPolygon(brush, points, FillMode.Winding);
                    }

                    graphPoints[i].Y = graphPoints[i + 1].Y;
                    graphColors[i] = graphColors[i+1];
                }
                graphPoints[i].X = segmentWidth * i;
                graphPoints[i].Y = graphHeight - (int)graphLine - (int)currentload;
                graphColors[i] = this.currentcolor;
            }
        }
    }
}
