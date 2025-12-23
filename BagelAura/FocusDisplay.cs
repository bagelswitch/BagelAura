using System;
using System.Drawing;
using AltUI.Forms;
using System.Timers;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.Text.Json.Nodes;

namespace BagelAura
{
    public partial class FocusDisplay : DarkForm
    {
        private static System.Timers.Timer giphyTimer;

        private string query = "sloth";
        private string gifUrl = "";
        private string giphyKey = "";

        
        protected override void WndProc(ref Message m)
        {
            const uint WM_DISPLAYCHANGE = 0x007e;
            // Listen for operating system messages. 
            switch (m.Msg)
            {
                case (int) WM_DISPLAYCHANGE:
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        
        public void Shutdown()
        {
            this.giphybox.Dispose();
        }

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
            InitializeComponent();

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
            TopLeft.X = TopLeft.X + 10;
            TopLeft.Y = TopLeft.Y + 1890;
            this.Location = TopLeft;
            this.Top = TopLeft.Y;
            this.Left = TopLeft.X;

            this.Visible = true;
            this.giphybox.Visible = true;

            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);

            this.Scale(new SizeF((float)0.999, (float)0.999));
        }

        public FocusDisplay()
        {
            InitializeLocation();

            this.giphybox.CoreWebView2InitializationCompleted += giphybox_CoreWebView2InitializationCompleted;

            this.giphybox.EnsureCoreWebView2Async();
            this.giphybox.Visible = true;

            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };

            restClient = new(handler)
            {
                BaseAddress = new Uri("https://tenor.googleapis.com/v2/search"),
                Timeout = TimeSpan.FromSeconds(10),
            };

            SetTimers();
        }

        private void giphybox_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.InitializationException != null)
            {
                Console.WriteLine("CoreWebView2 initialization failed: " + e.InitializationException.ToString());
            }
        }

        public void SetQuery(string query)
        {
            this.query = query;
        }

        private static HttpClient restClient;

        private string TenorGet(string query)
        {
            string queryString = "?q=" + query + "&random=false&limit=50&pos=0&ar_range=all&contentfilter=off&locale=en_US&key=" + this.giphyKey +"&client_key=bagelaura";
            var task = restClient.GetAsync(queryString);
            using HttpResponseMessage response = task.GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            HttpStatusCode responseCode = response.StatusCode;

            byte[] responseBytes = response.Content.ReadAsByteArrayAsync().Result;

            var responseJSON = JsonNode.Parse(responseBytes);
            var resultArray = responseJSON["results"].AsArray();
            int selector = new Random().Next(0, resultArray.Count);

            return (string) resultArray[selector].AsObject()["media_formats"].AsObject()["gif"].AsObject()["url"];
        }

        private string RandomGif(string query)
        {
            if (query.Equals("Google"))
            {
                query = "google it";
            }

            try
            {
                this.gifUrl = TenorGet(query);
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return "<html><body><div style=\"width: 100%;height: 100%;overflow: hidden;\"><img style=\"position: relative;top: 50%;left: 50%;transform: translate(-50%, -50%);height: 100%;width: auto;\" src=\"" + this.gifUrl + "\"></div></body></html>";
        }

        private void FocusDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
        }
    }
}
