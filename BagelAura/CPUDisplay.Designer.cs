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
            this.CPUMax = new System.Windows.Forms.Label();
            this.DiskC = new System.Windows.Forms.Label();
            this.DiskD = new System.Windows.Forms.Label();
            this.DiskE = new System.Windows.Forms.Label();
            this.DiskF = new System.Windows.Forms.Label();
            this.DiskW = new System.Windows.Forms.Label();
            this.Network = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CPUPct
            // 
            this.CPUPct.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CPUPct.AutoSize = true;
            this.CPUPct.BackColor = System.Drawing.Color.Transparent;
            this.CPUPct.Font = new System.Drawing.Font("Quartz MS", 30F);
            this.CPUPct.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.CPUPct.Location = new System.Drawing.Point(331, 75);
            this.CPUPct.Name = "CPUPct";
            this.CPUPct.Size = new System.Drawing.Size(100, 50);
            this.CPUPct.TabIndex = 0;
            this.CPUPct.Text = "00%";
            this.CPUPct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CPUMax
            // 
            this.CPUMax.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CPUMax.AutoSize = true;
            this.CPUMax.BackColor = System.Drawing.Color.Transparent;
            this.CPUMax.Font = new System.Drawing.Font("Quartz MS", 18F);
            this.CPUMax.ForeColor = System.Drawing.Color.Gray;
            this.CPUMax.Location = new System.Drawing.Point(270, 25);
            this.CPUMax.Name = "CPUMax";
            this.CPUMax.Size = new System.Drawing.Size(50, 25);
            this.CPUMax.TabIndex = 0;
            this.CPUMax.Text = "00%";
            this.CPUMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DiskC
            // 
            this.DiskC.AutoSize = true;
            this.DiskC.BackColor = System.Drawing.Color.Transparent;
            this.DiskC.Font = new System.Drawing.Font("Segoe Keycaps", 15F, System.Drawing.FontStyle.Bold);
            this.DiskC.ForeColor = System.Drawing.Color.LightGreen;
            this.DiskC.Location = new System.Drawing.Point(3, 1);
            this.DiskC.Name = "DiskC";
            this.DiskC.Size = new System.Drawing.Size(54, 35);
            this.DiskC.TabIndex = 1;
            this.DiskC.Text = "C";
            this.DiskC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DiskD
            // 
            this.DiskD.AutoSize = true;
            this.DiskD.BackColor = System.Drawing.Color.Transparent;
            this.DiskD.Font = new System.Drawing.Font("Segoe Keycaps", 15F, System.Drawing.FontStyle.Bold);
            this.DiskD.ForeColor = System.Drawing.Color.Yellow;
            this.DiskD.Location = new System.Drawing.Point(33, 1);
            this.DiskD.Name = "DiskD";
            this.DiskD.Size = new System.Drawing.Size(54, 35);
            this.DiskD.TabIndex = 1;
            this.DiskD.Text = "D";
            this.DiskD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DiskE
            // 
            this.DiskE.AutoSize = true;
            this.DiskE.BackColor = System.Drawing.Color.Transparent;
            this.DiskE.Font = new System.Drawing.Font("Segoe Keycaps", 15F, System.Drawing.FontStyle.Bold);
            this.DiskE.ForeColor = System.Drawing.Color.Yellow;
            this.DiskE.Location = new System.Drawing.Point(63, 1);
            this.DiskE.Name = "DiskE";
            this.DiskE.Size = new System.Drawing.Size(54, 35);
            this.DiskE.TabIndex = 1;
            this.DiskE.Text = "E";
            this.DiskE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DiskF
            // 
            this.DiskF.AutoSize = true;
            this.DiskF.BackColor = System.Drawing.Color.Transparent;
            this.DiskF.Font = new System.Drawing.Font("Segoe Keycaps", 15F, System.Drawing.FontStyle.Bold);
            this.DiskF.ForeColor = System.Drawing.Color.LightGreen;
            this.DiskF.Location = new System.Drawing.Point(93, 1);
            this.DiskF.Name = "DiskF";
            this.DiskF.Size = new System.Drawing.Size(54, 35);
            this.DiskF.TabIndex = 1;
            this.DiskF.Text = "F";
            this.DiskF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DiskW
            // 
            this.DiskW.AutoSize = true;
            this.DiskW.BackColor = System.Drawing.Color.Transparent;
            this.DiskW.Font = new System.Drawing.Font("Segoe Keycaps", 15F, System.Drawing.FontStyle.Bold);
            this.DiskW.ForeColor = System.Drawing.Color.LightGreen;
            this.DiskW.Location = new System.Drawing.Point(123, 1);
            this.DiskW.Name = "DiskW";
            this.DiskW.Size = new System.Drawing.Size(54, 35);
            this.DiskW.TabIndex = 1;
            this.DiskW.Text = "W";
            this.DiskW.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Network
            // 
            this.Network.AutoSize = true;
            this.Network.BackColor = System.Drawing.Color.Transparent;
            this.Network.Font = new System.Drawing.Font("Segoe Keycaps", 15F, System.Drawing.FontStyle.Bold);
            this.Network.ForeColor = System.Drawing.Color.LightGreen;
            this.Network.Location = new System.Drawing.Point(610, 1);
            this.Network.Name = "Network";
            this.Network.Size = new System.Drawing.Size(54, 35);
            this.Network.TabIndex = 1;
            this.Network.Text = "N";
            this.Network.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CPUDisplay
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.ClientSize = new System.Drawing.Size(662, 150);
            this.ControlBox = false;
            this.Controls.Add(this.DiskC);
            this.Controls.Add(this.DiskD);
            this.Controls.Add(this.DiskE);
            this.Controls.Add(this.DiskF);
            this.Controls.Add(this.DiskW);
            this.Controls.Add(this.Network);
            this.Controls.Add(this.CPUPct);
            this.Controls.Add(this.CPUMax);
            this.CornerStyle = AltUI.Forms.DarkForm.CornerPreference.Round;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CPUDisplay";
            this.Opacity = 0.75D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TransparencyKey = System.Drawing.Color.White;
            this.AllowTransparency = false;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CPUDisplay_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label CPUPct;
        private Label CPUMax;
        private Label DiskC;
        private Label DiskD;
        private Label DiskE;
        private Label DiskF;
        private Label DiskW;
        private Label Network;
    }
}