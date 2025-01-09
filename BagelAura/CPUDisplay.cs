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
        private int graphHeight = 150;

        public Color currentColor = Color.Black;
        public Color currentTextColor = Color.White;
        public int currentload = 0;

        public Point[] graphPoints = new Point[71];
        public Color[] graphColors = new Color[71];

        private int segmentWidth;

        public DriveStatus DriveCStatus = DriveStatus.Idle;
        public DriveStatus DriveDStatus = DriveStatus.Idle;
        public DriveStatus DriveEStatus = DriveStatus.Idle;
        public DriveStatus DriveFStatus = DriveStatus.Idle;

        public enum DriveStatus
        {
            Read,
            Write,
            Idle
        }

        private Color DriveStatusColor(DriveStatus status)
        {
            switch (status)
            {
                case DriveStatus.Write:
                    return Color.Yellow;
                case DriveStatus.Read:
                    return Color.LightGreen;
                default:
                    return Color.DarkGray;
            }
        }

        public CPUDisplay()
        {
            InitializeComponent();

            this.Bounds = new Rectangle(10, 0, graphWidth, graphHeight);
            this.TopMost = true;
            Point TopLeft = System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Location;
            TopLeft.X = TopLeft.X + 10;
            TopLeft.Y = TopLeft.Y + 2350;
            this.Location = TopLeft;

            this.CPUPct.Location = new System.Drawing.Point(
                                            (this.Width / 2) - (this.CPUPct.Width / 2),
                                            (this.Height / 2) - (this.CPUPct.Height / 2)); //= new System.Drawing.Point(430, 50);

            segmentWidth = graphWidth / graphPoints.Length;

            this.Visible = true;
            this.CPUPct.Visible = true;
            this.DiskC.Visible = true;
            this.DiskD.Visible = true;
            this.DiskE.Visible = true;
            this.DiskF.Visible = true;

            this.SetStyle(
                            System.Windows.Forms.ControlStyles.UserPaint |
                            System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                            System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true );
        }

        private void CPUDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (currentload < 0) currentload = 0;
            if (currentload > 100) currentload = 100;

            int maxVal = graphHeight;

            Point[] points = Array.Empty<Point>();

            Graphics gfx = e.Graphics;

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
            graphPoints[i].Y = graphHeight - ((currentload * graphHeight)/100); //graphHeight - currentload;
            graphColors[i] = this.currentColor;

            points = new Point[]{  new Point(graphPoints[i].X, graphPoints[i].Y),
                                    new Point(graphWidth, graphPoints[i].Y),
                                    new Point(graphWidth, graphHeight),
                                    new Point(graphPoints[i].X, graphHeight) };

            using (var brush = new SolidBrush(graphColors[i]))
            {
                gfx.FillPolygon(brush, points, FillMode.Winding);
            }

            if (maxVal < graphHeight)
            {
                using (var pen = new Pen(currentTextColor, 4))
                {
                    gfx.DrawLine(pen, new Point(0, maxVal), new Point(CPUPct.Left - 10, maxVal));
                    gfx.DrawLine(pen, new Point(CPUPct.Right + 10, maxVal), new Point(graphWidth, maxVal));
                    pen.DashPattern = new float[] { 3, 5 };
                    pen.Color = Color.DarkGray;
                    pen.Width = 1;
                    gfx.DrawLine(pen, new Point(0, (int)((float)(graphHeight) * 0.25)), new Point(graphWidth, (int)((float)(graphHeight) * 0.25)));
                    gfx.DrawLine(pen, new Point(0, (int)((float)(graphHeight) * 0.50)), new Point(graphWidth, (int)((float)(graphHeight) * 0.50)));
                    gfx.DrawLine(pen, new Point(0, (int)((float)(graphHeight) * 0.75)), new Point(graphWidth, (int)((float)(graphHeight) * 0.75)));
                }
            }
            maxVal = ((graphHeight - maxVal)*100)/graphHeight;
            this.CPUPct.Text = maxVal.ToString("00") + "%";
            this.CPUPct.ForeColor = currentTextColor;

            this.DiskC.ForeColor = DriveStatusColor(this.DriveCStatus);
            this.DiskD.ForeColor = DriveStatusColor(this.DriveDStatus);
            this.DiskE.ForeColor = DriveStatusColor(this.DriveEStatus);
            this.DiskF.ForeColor = DriveStatusColor(this.DriveFStatus);
        }
    }
}
