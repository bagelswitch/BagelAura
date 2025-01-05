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
using Vanara.PInvoke;

namespace BagelAura
{
    public partial class CPUDisplay : DarkForm
    {
        private int graphWidth = 713;
        private int graphHeight = 140;
        private float graphLine = 15.0F;

        public Color currentColor = Color.Black;
        public Color currentTextColor = Color.White;
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

            this.Visible = true;
            this.CPUPct.Visible= true;
        }

        private void CPUDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (currentload < 0) currentload = 0;
            if (currentload > 100) currentload = 100;

            int maxVal = graphHeight;

            Point[] points = Array.Empty<Point>();

            using (var gfx = e.Graphics)
            {
                int i = 0;
                for (; i < graphPoints.Length - 1; i++)
                {
                    graphPoints[i].X = segmentWidth * i;

                    points = new Point[]{ new Point(graphPoints[i].X, graphPoints[i].Y), 
                                        new Point(graphPoints[i + 1].X, graphPoints[i + 1].Y), 
                                        new Point(graphPoints[i + 1].X, graphHeight),
                                        new Point(graphPoints[i].X, graphHeight) };

                    using (var brush = new SolidBrush(graphColors[i]))
                    {
                        gfx.FillPolygon(brush, points, FillMode.Winding);
                    }

                    graphPoints[i].Y = graphPoints[i + 1].Y;
                    graphColors[i] = graphColors[i+1];

                    if(graphPoints[i+1].Y < maxVal) maxVal = graphPoints[i+1].Y;
                }
                graphPoints[i].X = segmentWidth * i;
                graphPoints[i].Y = graphHeight - (int)graphLine - (int)currentload;
                graphColors[i] = this.currentColor;

                points = new Point[]{  new Point(graphPoints[i].X, graphPoints[i].Y),
                                        new Point(graphWidth, graphPoints[i].Y),
                                        new Point(graphWidth, graphHeight),
                                        new Point(graphPoints[i].X, graphHeight) };

                using (var brush = new SolidBrush(graphColors[i]))
                {
                    gfx.FillPolygon(brush, points, FillMode.Winding);
                }

                using (var pen = new Pen(currentTextColor, 4))
                {
                    gfx.DrawLine(pen, new Point(0, maxVal), new Point(CPUPct.Left - 10, maxVal));
                    gfx.DrawLine(pen, new Point(CPUPct.Right + 10, maxVal), new Point(graphWidth, maxVal));
                }
                maxVal = graphHeight - (maxVal + (int)graphLine);
                this.CPUPct.Text = maxVal.ToString("00") + "%";
                this.CPUPct.ForeColor = currentTextColor;
            }
        }
    }
}
