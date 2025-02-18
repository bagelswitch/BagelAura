using System;
using System.Drawing;
using AltUI.Forms;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using System.Timers;
using System.Windows.Forms;
using System.Drawing.Printing;
using Vanara.InteropServices;
using System.Threading.Tasks;

namespace BagelAura
{
    public partial class FocusDisplay : DarkForm
    {
        private static System.Timers.Timer giphyTimer;

        private string query = "sloth";
        private string gifUrl = "";
        private string giphyKey = "";

        public void SetGiphyKey(string key)
        {
            this.giphyKey = key;
        }

        private void SetTimers()
        {
            giphyTimer = new System.Timers.Timer(45000);

            giphyTimer.Elapsed += OnGiphyEvent;
            giphyTimer.AutoReset = true;
            giphyTimer.Enabled = true;
        }

        private void OnGiphyEvent(Object source, ElapsedEventArgs e)
        {
            string html = RandomGif(this.query);

            this.queryTerm.Text = this.query;
            this.Invalidate();
            this.Update();

            //Console.WriteLine("giphybox html: " + html);

            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    this.giphybox.NavigateToString(html);
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            this.giphybox.Visible = true;

            //Console.WriteLine("giphybox html updated");
        }

        public void InitializeLocation()
        {
            this.Bounds = new Rectangle(10, 0, 662, 500);
            Point TopLeft = new Point(0, 0);
            if (System.Windows.Forms.Screen.AllScreens[0].Primary)
            {
                TopLeft = System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Location;
            }
            else
            {
                TopLeft = System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Location;
            }
            TopLeft.X = TopLeft.X + 350;
            TopLeft.Y = TopLeft.Y + 1890;
            this.Location = TopLeft;
            this.Top = TopLeft.Y;
            this.Left = TopLeft.X;

            this.Visible = true;
            this.giphybox.Visible = true;
        }

        public FocusDisplay()
        {
            InitializeComponent();

            InitializeLocation();

            this.SetStyle(
                            System.Windows.Forms.ControlStyles.UserPaint |
                            System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                            System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);

            this.giphybox.CoreWebView2InitializationCompleted += giphybox_CoreWebView2InitializationCompleted;

            this.giphybox.EnsureCoreWebView2Async();
            this.giphybox.Visible = true;

            SetTimers();
        }

        private void giphybox_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.InitializationException != null)
            {
                Console.WriteLine("CoreWebView2 initialization failed: " + e.InitializationException.ToString());
            }
            //Console.WriteLine("giphybox init completed");
        }

        public void SetQuery(string query)
        {
            this.query = query;

            //Console.WriteLine("Set active window title: " + query);
        }

        private string RandomGif(string query)
        {
            if(query.Equals("Google"))
            {
                query = "google it";
            }

            var giphy = new Giphy(this.giphyKey);
            var searchParameter = new SearchParameter()
            {
                Query = query,
                Limit = 50
            };
            var task = giphy.GifSearch(searchParameter);
            var gifResult = task.GetAwaiter().GetResult();

            int selector = new Random().Next(0, gifResult.Data.Length);
            this.gifUrl = gifResult.Data[selector].Images.FixedHeight.Webp;

            //Console.WriteLine("Setting gif URL: " + this.gifUrl);

            return "<html><body><div style=\"width: 100%;height: 100%;overflow: hidden;\"><img style=\"position: relative;top: 50%;left: 50%;transform: translate(-50%, -50%);height: 100%;width: auto;\" src=\"" + this.gifUrl + "\"></div></body></html>";
        }

        private void FocusDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
        }
    }
}
