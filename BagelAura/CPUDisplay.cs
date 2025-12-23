using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AltUI.Forms;

namespace BagelAura
{
    public partial class CPUDisplay : DarkForm
    {
        public bool isDirty = false;

        private int graphWidth = 662;
        private int graphHeight = 150;

        public Color currentColor = Color.Black;
        public int currentload = 0;

        static SimpleMovingAverage lineCalculator = new SimpleMovingAverage(k: 20);

        public Point[] graphPoints = new Point[66];
        public Color[] graphColors = new Color[66];

        private int segmentWidth;

        public DriveStatus DriveCStatus = DriveStatus.Idle;
        public DriveStatus DriveDStatus = DriveStatus.Idle;
        public DriveStatus DriveEStatus = DriveStatus.Idle;
        public DriveStatus DriveFStatus = DriveStatus.Idle;
        public DriveStatus DriveWStatus = DriveStatus.Idle;

        public NetworkStatus NetStatus = NetworkStatus.Idle; 

        public enum DriveStatus
        {
            Read,
            Write,
            Idle
        }

        public enum NetworkStatus
        {
            Send,
            Receive,
            Idle
        }

        protected override void WndProc(ref Message m)
        {
            const uint WM_DISPLAYCHANGE = 0x007e;
            // Listen for operating system messages. 
            switch (m.Msg)
            {
                case (int)WM_DISPLAYCHANGE:
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private Color DriveStatusColor(DriveStatus status)
        {
            switch (status)
            {
                case DriveStatus.Write:
                    return Color.Orange;
                case DriveStatus.Read:
                    return Color.LightGreen;
                default:
                    return Color.DarkGray;
            }
        }

        private Color NetworkStatusColor(NetworkStatus status)
        {
            switch (status)
            {
                case NetworkStatus.Send:
                    return Color.Orange;
                case NetworkStatus.Receive:
                    return Color.LightGreen;
                default:
                    return Color.DarkGray;
            }
            }

        public void InitializeLocation()
        {
            InitializeComponent();

            this.Bounds = new Rectangle(10, 0, graphWidth, graphHeight);
            Point TopLeft = new Point(0, 0);
            if (System.Windows.Forms.Screen.AllScreens[0].Primary)
            {
            //Console.WriteLine("CPUDisplay: Using screen 1");
                TopLeft = System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Location;
            }
            else
            {
            //Console.WriteLine("CPUDisplay: Using screen 0");
                TopLeft = System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Location;
            }
            TopLeft.X = TopLeft.X + 10;
            TopLeft.Y = TopLeft.Y + 2400;
            this.Location = TopLeft;
            this.Top = TopLeft.Y;
            this.Left = TopLeft.X;

            //Console.WriteLine("CPUDisplay: Using origin:  X: " + this.Left + "  Y: " + this.Top);

            this.CPUPct.Location = new System.Drawing.Point(
                                            (this.Width / 2) - (this.CPUPct.Width / 2),
                                            (this.Height / 2) - (this.CPUPct.Height / 2)); //= new System.Drawing.Point(430, 50);

            this.CPUMax.Location = new System.Drawing.Point(
                                this.CPUPct.Right + 20,
                                this.CPUPct.Top - 30);

            segmentWidth = graphWidth / graphPoints.Length;

            this.Visible = true;
            this.CPUPct.Visible = true;
            this.CPUMax.Visible = true;
            this.DiskC.Visible = true;
            this.DiskD.Visible = true;
            this.DiskE.Visible = true;
            this.DiskF.Visible = true;
            this.DiskW.Visible = true;
            this.Network.Visible = true;

            this.SetStyle(
                            System.Windows.Forms.ControlStyles.UserPaint |
                            System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                            System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);

            this.Scale(new SizeF((float)0.999, (float)0.999));
        }

        public CPUDisplay()
        {
            InitializeLocation();
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

                using (var brush = new LinearGradientBrush(new Point(0, 0), new Point(1, 0), graphColors[i], graphColors[i + 1]))
                {
                    gfx.FillPolygon(brush, points, FillMode.Winding);
                }
                if (this.isDirty)
                {
                    graphPoints[i].Y = graphPoints[i + 1].Y;
                    graphColors[i] = graphColors[i + 1];
                }

                if (graphPoints[i + 1].Y < maxVal) maxVal = graphPoints[i + 1].Y;
            }

            this.isDirty = false;

            graphPoints[i].X = segmentWidth * i;
            graphPoints[i].Y = graphHeight - ((currentload * graphHeight) / 100);
            graphColors[i] = this.currentColor;

            points = new Point[]{  new Point(graphPoints[i].X, graphPoints[i].Y),
                                new Point(graphWidth, graphPoints[i].Y),
                                new Point(graphWidth, graphHeight),
                                new Point(graphPoints[i].X, graphHeight) };

            using (var brush = new SolidBrush(graphColors[i]))
            {
                gfx.FillPolygon(brush, points, FillMode.Winding);
            }

            int lineSMA = lineCalculator.Update(graphPoints[i].Y);

            using (var pen = new Pen(Color.LightGray, 3))
            {
                if (graphPoints[i].Y < graphHeight)
                {
                    gfx.DrawLine(pen, new Point(0, lineSMA), new Point(CPUPct.Left - 10, lineSMA));
                    gfx.DrawLine(pen, new Point(CPUPct.Right + 10, lineSMA), new Point(graphWidth, lineSMA));
                }

                pen.Color = Color.Gray;

                if (maxVal < graphHeight)
                {
                    gfx.DrawLine(pen, new Point(0, maxVal), new Point(CPUPct.Left - 10, maxVal));
                    gfx.DrawLine(pen, new Point(CPUPct.Right + 10, maxVal), new Point(graphWidth, maxVal));
                }

                pen.DashPattern = new float[] { 3, 5 };
                pen.Color = Color.DarkGray;
                pen.Width = 1;
                gfx.DrawLine(pen, new Point(0, (int)((float)(graphHeight) * 0.25)), new Point(graphWidth, (int)((float)(graphHeight) * 0.25)));
                gfx.DrawLine(pen, new Point(0, (int)((float)(graphHeight) * 0.50)), new Point(graphWidth, (int)((float)(graphHeight) * 0.50)));
                gfx.DrawLine(pen, new Point(0, (int)((float)(graphHeight) * 0.75)), new Point(graphWidth, (int)((float)(graphHeight) * 0.75)));
            }
            lineSMA = ((graphHeight - lineSMA) * 100) / graphHeight;
            this.CPUPct.Text = lineSMA.ToString("00") + "%";
            this.CPUPct.ForeColor = Color.LightGray;

            maxVal = ((graphHeight - maxVal) * 100) / graphHeight;
            this.CPUMax.Text = maxVal.ToString("00") + "%";
            this.CPUMax.ForeColor = Color.Gray;

            this.DiskC.ForeColor = DriveStatusColor(this.DriveCStatus);
            this.DiskD.ForeColor = DriveStatusColor(this.DriveDStatus);
            this.DiskE.ForeColor = DriveStatusColor(this.DriveEStatus);
            this.DiskF.ForeColor = DriveStatusColor(this.DriveFStatus);
            this.DiskW.ForeColor = DriveStatusColor(this.DriveWStatus);

            this.Network.ForeColor = NetworkStatusColor(this.NetStatus);
        }
    }
}
