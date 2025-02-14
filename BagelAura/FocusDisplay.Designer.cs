using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using AltUI.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace BagelAura
{
    partial class FocusDisplay : DarkForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.giphybox = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.giphybox)).BeginInit();
            this.SuspendLayout();
            // 
            // giphybox
            // 
            this.giphybox.AllowExternalDrop = false;
            this.giphybox.CreationProperties = null;
            this.giphybox.DefaultBackgroundColor = System.Drawing.Color.Transparent;
            this.giphybox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.giphybox.Location = new System.Drawing.Point(0, 0);
            this.giphybox.MinimumSize = new System.Drawing.Size(713, 200);
            this.giphybox.Name = "giphybox";
            this.giphybox.Size = new System.Drawing.Size(713, 200);
            this.giphybox.TabIndex = 0;
            this.giphybox.TabStop = false;
            this.giphybox.ZoomFactor = 1D;
            // 
            // FocusDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.ClientSize = new System.Drawing.Size(713, 200);
            this.ControlBox = false;
            this.Controls.Add(this.giphybox);
            this.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.Round;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FocusDisplay";
            this.Opacity = 0.75D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TransparencyKey = System.Drawing.Color.White;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FocusDisplay_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.giphybox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private WebView2 giphybox;
    }
}