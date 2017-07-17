namespace ConfigCore.UI
{
	partial class FormMessage
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessage));
			this.tbMessage = new System.Windows.Forms.RichTextBox();
			this.tbDetalis = new System.Windows.Forms.RichTextBox();
			this.btnSthowStack = new System.Windows.Forms.LinkLabel();
			this.btnShowMessage = new System.Windows.Forms.LinkLabel();
			this.btnShowScript = new System.Windows.Forms.LinkLabel();
			this.icons = new System.Windows.Forms.ImageList(this.components);
			this.icon = new System.Windows.Forms.PictureBox();
			this.buttonsPanel = new System.Windows.Forms.Panel();
			this.btnHide = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
			this.SuspendLayout();
			// 
			// tbMessage
			// 
			this.tbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbMessage.BackColor = System.Drawing.Color.White;
			this.tbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbMessage.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.tbMessage.DetectUrls = false;
			this.tbMessage.HideSelection = false;
			this.tbMessage.Location = new System.Drawing.Point(66, 12);
			this.tbMessage.Name = "tbMessage";
			this.tbMessage.ReadOnly = true;
			this.tbMessage.Size = new System.Drawing.Size(384, 55);
			this.tbMessage.TabIndex = 500;
			this.tbMessage.Text = "";
			// 
			// tbDetalis
			// 
			this.tbDetalis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDetalis.BackColor = System.Drawing.Color.White;
			this.tbDetalis.Location = new System.Drawing.Point(10, 89);
			this.tbDetalis.Name = "tbDetalis";
			this.tbDetalis.ReadOnly = true;
			this.tbDetalis.Size = new System.Drawing.Size(438, 189);
			this.tbDetalis.TabIndex = 505;
			this.tbDetalis.Text = "";
			this.tbDetalis.WordWrap = false;
			// 
			// btnSthowStack
			// 
			this.btnSthowStack.AutoSize = true;
			this.btnSthowStack.BackColor = System.Drawing.Color.Transparent;
			this.btnSthowStack.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnSthowStack.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.btnSthowStack.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
			this.btnSthowStack.Location = new System.Drawing.Point(147, 70);
			this.btnSthowStack.Name = "btnSthowStack";
			this.btnSthowStack.Size = new System.Drawing.Size(94, 14);
			this.btnSthowStack.TabIndex = 502;
			this.btnSthowStack.TabStop = true;
			this.btnSthowStack.Text = "Show Call Stack";
			this.btnSthowStack.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnSthowStack_LinkClicked);
			// 
			// btnShowMessage
			// 
			this.btnShowMessage.AutoSize = true;
			this.btnShowMessage.BackColor = System.Drawing.Color.Transparent;
			this.btnShowMessage.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnShowMessage.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.btnShowMessage.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
			this.btnShowMessage.Location = new System.Drawing.Point(10, 70);
			this.btnShowMessage.Name = "btnShowMessage";
			this.btnShowMessage.Size = new System.Drawing.Size(131, 14);
			this.btnShowMessage.TabIndex = 501;
			this.btnShowMessage.TabStop = true;
			this.btnShowMessage.Text = "Show Inner Messages";
			this.btnShowMessage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnShowMessage_LinkClicked);
			// 
			// btnShowScript
			// 
			this.btnShowScript.AutoSize = true;
			this.btnShowScript.BackColor = System.Drawing.Color.Transparent;
			this.btnShowScript.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnShowScript.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.btnShowScript.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
			this.btnShowScript.Location = new System.Drawing.Point(247, 70);
			this.btnShowScript.Name = "btnShowScript";
			this.btnShowScript.Size = new System.Drawing.Size(73, 14);
			this.btnShowScript.TabIndex = 503;
			this.btnShowScript.TabStop = true;
			this.btnShowScript.Text = "Show Script";
			this.btnShowScript.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnShowScript_LinkClicked);
			// 
			// icons
			// 
			this.icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("icons.ImageStream")));
			this.icons.TransparentColor = System.Drawing.Color.Transparent;
			this.icons.Images.SetKeyName(0, "Information");
			this.icons.Images.SetKeyName(1, "Warning");
			this.icons.Images.SetKeyName(2, "Error");
			// 
			// icon
			// 
			this.icon.Location = new System.Drawing.Point(12, 12);
			this.icon.Name = "icon";
			this.icon.Size = new System.Drawing.Size(48, 48);
			this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.icon.TabIndex = 506;
			this.icon.TabStop = false;
			// 
			// buttonsPanel
			// 
			this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonsPanel.Location = new System.Drawing.Point(0, 281);
			this.buttonsPanel.Name = "buttonsPanel";
			this.buttonsPanel.Size = new System.Drawing.Size(462, 37);
			this.buttonsPanel.TabIndex = 507;
			// 
			// btnHide
			// 
			this.btnHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnHide.AutoSize = true;
			this.btnHide.BackColor = System.Drawing.Color.Transparent;
			this.btnHide.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnHide.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.btnHide.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
			this.btnHide.Location = new System.Drawing.Point(419, 70);
			this.btnHide.Name = "btnHide";
			this.btnHide.Size = new System.Drawing.Size(31, 14);
			this.btnHide.TabIndex = 508;
			this.btnHide.TabStop = true;
			this.btnHide.Text = "Hide";
			this.btnHide.Visible = false;
			this.btnHide.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnHide_LinkClicked);
			// 
			// FormMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(462, 318);
			this.Controls.Add(this.btnHide);
			this.Controls.Add(this.buttonsPanel);
			this.Controls.Add(this.icon);
			this.Controls.Add(this.btnShowScript);
			this.Controls.Add(this.btnShowMessage);
			this.Controls.Add(this.btnSthowStack);
			this.Controls.Add(this.tbDetalis);
			this.Controls.Add(this.tbMessage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMessage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Acumatica Workspace Manager";
            this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox tbMessage;
		private System.Windows.Forms.RichTextBox tbDetalis;
		private System.Windows.Forms.LinkLabel btnSthowStack;
		private System.Windows.Forms.LinkLabel btnShowMessage;
		private System.Windows.Forms.LinkLabel btnShowScript;
		private System.Windows.Forms.ImageList icons;
		private System.Windows.Forms.PictureBox icon;
		private System.Windows.Forms.Panel buttonsPanel;
		private System.Windows.Forms.LinkLabel btnHide;
	}
}