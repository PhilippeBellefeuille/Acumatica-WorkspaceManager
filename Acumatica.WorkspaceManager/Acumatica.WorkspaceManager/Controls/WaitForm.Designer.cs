namespace ConfigCore.UI
{
	partial class WaitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitForm));
            this.lbWait = new System.Windows.Forms.Label();
            this.Spinner = new System.Windows.Forms.PictureBox();
            this.WaitProgressLabel = new System.Windows.Forms.Label();
            this.WaitProgressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.Spinner)).BeginInit();
            this.SuspendLayout();
            // 
            // lbWait
            // 
            this.lbWait.BackColor = System.Drawing.Color.Transparent;
            this.lbWait.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.lbWait.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.lbWait.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbWait.Location = new System.Drawing.Point(101, 15);
            this.lbWait.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbWait.Name = "lbWait";
            this.lbWait.Size = new System.Drawing.Size(262, 45);
            this.lbWait.TabIndex = 5;
            this.lbWait.Text = "Please Wait...";
            this.lbWait.UseWaitCursor = true;
            // 
            // Spinner
            // 
            this.Spinner.Image = ((System.Drawing.Image)(resources.GetObject("Spinner.Image")));
            this.Spinner.Location = new System.Drawing.Point(351, 10);
            this.Spinner.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Spinner.Name = "Spinner";
            this.Spinner.Size = new System.Drawing.Size(32, 32);
            this.Spinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.Spinner.TabIndex = 6;
            this.Spinner.TabStop = false;
            this.Spinner.UseWaitCursor = true;
            // 
            // WaitProgressLabel
            // 
            this.WaitProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WaitProgressLabel.BackColor = System.Drawing.Color.Transparent;
            this.WaitProgressLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WaitProgressLabel.ForeColor = System.Drawing.Color.Black;
            this.WaitProgressLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.WaitProgressLabel.Location = new System.Drawing.Point(13, 121);
            this.WaitProgressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WaitProgressLabel.Name = "WaitProgressLabel";
            this.WaitProgressLabel.Size = new System.Drawing.Size(459, 28);
            this.WaitProgressLabel.TabIndex = 26;
            this.WaitProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.WaitProgressLabel.UseWaitCursor = true;
            // 
            // WaitProgressBar
            // 
            this.WaitProgressBar.Location = new System.Drawing.Point(109, 84);
            this.WaitProgressBar.Name = "WaitProgressBar";
            this.WaitProgressBar.Size = new System.Drawing.Size(274, 23);
            this.WaitProgressBar.TabIndex = 27;
            this.WaitProgressBar.UseWaitCursor = true;
            // 
            // WaitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(485, 84);
            this.Controls.Add(this.WaitProgressBar);
            this.Controls.Add(this.WaitProgressLabel);
            this.Controls.Add(this.Spinner);
            this.Controls.Add(this.lbWait);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Acumatica Workspace Manager";
            this.UseWaitCursor = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaitForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.Spinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbWait;
		private System.Windows.Forms.PictureBox Spinner;
        private System.Windows.Forms.Label WaitProgressLabel;
        private System.Windows.Forms.ProgressBar WaitProgressBar;
    }
}