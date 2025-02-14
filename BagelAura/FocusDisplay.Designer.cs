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
            this.queryTerm = new System.Windows.Forms.Label();
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
            this.giphybox.MinimumSize = new System.Drawing.Size(713, 500);
            this.giphybox.Name = "giphybox";
            this.giphybox.Size = new System.Drawing.Size(713, 500);
            this.giphybox.TabIndex = 0;
            this.giphybox.TabStop = false;
            this.giphybox.ZoomFactor = 1D;
            // 
            // queryTerm
            // 
            this.queryTerm.AutoSize = true;
            this.queryTerm.BackColor = System.Drawing.Color.Transparent;
            this.queryTerm.Font = new System.Drawing.Font("Unispace", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.queryTerm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.queryTerm.Location = new System.Drawing.Point(13, 13);
            this.queryTerm.Name = "queryTerm";
            this.queryTerm.Size = new System.Drawing.Size(88, 29);
            this.queryTerm.TabIndex = 1;
            this.queryTerm.Text = "sloth";
            // 
            // FocusDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.ClientSize = new System.Drawing.Size(713, 500);
            this.ControlBox = false;
            this.Controls.Add(this.queryTerm);
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
            this.PerformLayout();

        }

        #endregion

        private WebView2 giphybox;
        private Label queryTerm;
    }
}