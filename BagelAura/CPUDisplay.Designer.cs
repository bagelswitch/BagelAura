using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using AltUI.Forms;

namespace BagelAura
{
    partial class CPUDisplay : DarkForm
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
            this.CPUPct = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CPUPct
            // 
            this.CPUPct.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CPUPct.AutoSize = true;
            this.CPUPct.Font = new System.Drawing.Font("Quartz MS", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CPUPct.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.CPUPct.BackColor = System.Drawing.Color.Transparent;
            this.CPUPct.Name = "CPUPct";
            this.CPUPct.Size = new System.Drawing.Size(206, 96);
            this.CPUPct.TabIndex = 0;
            this.CPUPct.Text = "00%";
            this.CPUPct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CPUDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.ClientSize = new System.Drawing.Size(889, 562);
            this.ControlBox = false;
            this.Controls.Add(this.CPUPct);
            this.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.Round;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CPUDisplay";
            this.Opacity = 0.75D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TransparencyKey = System.Drawing.Color.White;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CPUDisplay_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label CPUPct;
    }
}