namespace Acumatica.WorkspaceManager
{
    partial class Main
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.BuildPackageDataGridView = new System.Windows.Forms.DataGridView();
            this.majorVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.minorVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isRemoteDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.isLocalDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.keyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BuildPackageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.ReloadButton = new System.Windows.Forms.Button();
            this.VersionMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.InstallButton = new System.Windows.Forms.Button();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.UninstallButton = new System.Windows.Forms.Button();
            this.ShowRemoteCheckBox = new System.Windows.Forms.CheckBox();
            this.ShowLocalCheckBox = new System.Windows.Forms.CheckBox();
            this.OpenFolderButton = new System.Windows.Forms.Button();
            this.LaunchButton = new System.Windows.Forms.Button();
            this.AcumaticaLinkLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.BuildPackageDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuildPackageBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // BuildPackageDataGridView
            // 
            this.BuildPackageDataGridView.AllowUserToAddRows = false;
            this.BuildPackageDataGridView.AllowUserToDeleteRows = false;
            this.BuildPackageDataGridView.AllowUserToOrderColumns = true;
            this.BuildPackageDataGridView.AllowUserToResizeRows = false;
            this.BuildPackageDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BuildPackageDataGridView.AutoGenerateColumns = false;
            this.BuildPackageDataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.BuildPackageDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.BuildPackageDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BuildPackageDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.majorVersionDataGridViewTextBoxColumn,
            this.minorVersionDataGridViewTextBoxColumn,
            this.buildNumberDataGridViewTextBoxColumn,
            this.isRemoteDataGridViewCheckBoxColumn,
            this.isLocalDataGridViewCheckBoxColumn,
            this.keyDataGridViewTextBoxColumn});
            this.BuildPackageDataGridView.DataBindings.Add(new System.Windows.Forms.Binding("Tag", this.BuildPackageBindingSource, "Key", true));
            this.BuildPackageDataGridView.DataSource = this.BuildPackageBindingSource;
            this.BuildPackageDataGridView.Location = new System.Drawing.Point(21, 98);
            this.BuildPackageDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BuildPackageDataGridView.MultiSelect = false;
            this.BuildPackageDataGridView.Name = "BuildPackageDataGridView";
            this.BuildPackageDataGridView.RowHeadersVisible = false;
            this.BuildPackageDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BuildPackageDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.BuildPackageDataGridView.Size = new System.Drawing.Size(1042, 468);
            this.BuildPackageDataGridView.TabIndex = 0;
            // 
            // majorVersionDataGridViewTextBoxColumn
            // 
            this.majorVersionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.majorVersionDataGridViewTextBoxColumn.DataPropertyName = "MajorVersion";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.majorVersionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.majorVersionDataGridViewTextBoxColumn.FillWeight = 1F;
            this.majorVersionDataGridViewTextBoxColumn.HeaderText = "Major Version";
            this.majorVersionDataGridViewTextBoxColumn.Name = "majorVersionDataGridViewTextBoxColumn";
            this.majorVersionDataGridViewTextBoxColumn.ReadOnly = true;
            this.majorVersionDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // minorVersionDataGridViewTextBoxColumn
            // 
            this.minorVersionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.minorVersionDataGridViewTextBoxColumn.DataPropertyName = "MinorVersion";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.minorVersionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.minorVersionDataGridViewTextBoxColumn.FillWeight = 1F;
            this.minorVersionDataGridViewTextBoxColumn.HeaderText = "Minor Version";
            this.minorVersionDataGridViewTextBoxColumn.Name = "minorVersionDataGridViewTextBoxColumn";
            this.minorVersionDataGridViewTextBoxColumn.ReadOnly = true;
            this.minorVersionDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // buildNumberDataGridViewTextBoxColumn
            // 
            this.buildNumberDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.buildNumberDataGridViewTextBoxColumn.DataPropertyName = "BuildNumber";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.buildNumberDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.buildNumberDataGridViewTextBoxColumn.FillWeight = 1F;
            this.buildNumberDataGridViewTextBoxColumn.HeaderText = "Build Number";
            this.buildNumberDataGridViewTextBoxColumn.Name = "buildNumberDataGridViewTextBoxColumn";
            this.buildNumberDataGridViewTextBoxColumn.ReadOnly = true;
            this.buildNumberDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // isRemoteDataGridViewCheckBoxColumn
            // 
            this.isRemoteDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.isRemoteDataGridViewCheckBoxColumn.DataPropertyName = "IsRemote";
            this.isRemoteDataGridViewCheckBoxColumn.HeaderText = "Remote";
            this.isRemoteDataGridViewCheckBoxColumn.Name = "isRemoteDataGridViewCheckBoxColumn";
            this.isRemoteDataGridViewCheckBoxColumn.ReadOnly = true;
            this.isRemoteDataGridViewCheckBoxColumn.Width = 70;
            // 
            // isLocalDataGridViewCheckBoxColumn
            // 
            this.isLocalDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.isLocalDataGridViewCheckBoxColumn.DataPropertyName = "IsLocal";
            this.isLocalDataGridViewCheckBoxColumn.HeaderText = "Local";
            this.isLocalDataGridViewCheckBoxColumn.Name = "isLocalDataGridViewCheckBoxColumn";
            this.isLocalDataGridViewCheckBoxColumn.ReadOnly = true;
            this.isLocalDataGridViewCheckBoxColumn.Width = 70;
            // 
            // keyDataGridViewTextBoxColumn
            // 
            this.keyDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.keyDataGridViewTextBoxColumn.DataPropertyName = "Key";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.keyDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.keyDataGridViewTextBoxColumn.HeaderText = "Key";
            this.keyDataGridViewTextBoxColumn.Name = "keyDataGridViewTextBoxColumn";
            this.keyDataGridViewTextBoxColumn.ReadOnly = true;
            this.keyDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // BuildPackageBindingSource
            // 
            this.BuildPackageBindingSource.DataSource = typeof(Acumatica.WorkspaceManager.Builds.BuildPackage);
            this.BuildPackageBindingSource.Filter = "MajorVersion, MinorVersion";
            this.BuildPackageBindingSource.Sort = "MajorVersion, MinorVersion, BuildNumber";
            this.BuildPackageBindingSource.CurrentChanged += new System.EventHandler(this.BuildPackageBindingSource_CurrentChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(561, 48);
            this.label1.TabIndex = 24;
            this.label1.Text = "Workspace Manager";
            // 
            // label46
            // 
            this.label46.BackColor = System.Drawing.Color.Transparent;
            this.label46.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label46.ForeColor = System.Drawing.Color.Black;
            this.label46.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label46.Location = new System.Drawing.Point(17, 57);
            this.label46.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(994, 42);
            this.label46.TabIndex = 25;
            this.label46.Text = "Using this screen, you can download and manage Acumatica ERP installations.";
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.Color.Transparent;
            this.label24.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.label24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label24.Location = new System.Drawing.Point(17, 582);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(125, 23);
            this.label24.TabIndex = 26;
            this.label24.Text = "Version filter:";
            // 
            // ReloadButton
            // 
            this.ReloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ReloadButton.BackColor = System.Drawing.SystemColors.Control;
            this.ReloadButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.ReloadButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ReloadButton.Location = new System.Drawing.Point(917, 53);
            this.ReloadButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(146, 35);
            this.ReloadButton.TabIndex = 121;
            this.ReloadButton.Text = "&Reload the List";
            this.ReloadButton.UseVisualStyleBackColor = true;
            this.ReloadButton.Click += new System.EventHandler(this.ReloadDataEventHandler);
            // 
            // VersionMaskedTextBox
            // 
            this.VersionMaskedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.VersionMaskedTextBox.Location = new System.Drawing.Point(149, 582);
            this.VersionMaskedTextBox.Mask = "9.99.9999";
            this.VersionMaskedTextBox.Name = "VersionMaskedTextBox";
            this.VersionMaskedTextBox.Size = new System.Drawing.Size(129, 26);
            this.VersionMaskedTextBox.TabIndex = 1003;
            this.VersionMaskedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.VersionMaskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
            this.VersionMaskedTextBox.Leave += new System.EventHandler(this.ReloadDataEventHandler);
            // 
            // InstallButton
            // 
            this.InstallButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.InstallButton.BackColor = System.Drawing.SystemColors.Control;
            this.InstallButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.InstallButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.InstallButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.InstallButton.Location = new System.Drawing.Point(772, 576);
            this.InstallButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Size = new System.Drawing.Size(138, 37);
            this.InstallButton.TabIndex = 1004;
            this.InstallButton.Text = "&Install";
            this.InstallButton.UseVisualStyleBackColor = true;
            this.InstallButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // DownloadButton
            // 
            this.DownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadButton.BackColor = System.Drawing.SystemColors.Control;
            this.DownloadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DownloadButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.DownloadButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DownloadButton.Location = new System.Drawing.Point(619, 576);
            this.DownloadButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(138, 37);
            this.DownloadButton.TabIndex = 1005;
            this.DownloadButton.Text = "&Download";
            this.DownloadButton.UseVisualStyleBackColor = true;
            this.DownloadButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // UninstallButton
            // 
            this.UninstallButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UninstallButton.BackColor = System.Drawing.SystemColors.Control;
            this.UninstallButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.UninstallButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.UninstallButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UninstallButton.Location = new System.Drawing.Point(772, 623);
            this.UninstallButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UninstallButton.Name = "UninstallButton";
            this.UninstallButton.Size = new System.Drawing.Size(138, 37);
            this.UninstallButton.TabIndex = 1006;
            this.UninstallButton.Text = "&Uninstall";
            this.UninstallButton.UseVisualStyleBackColor = true;
            this.UninstallButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // ShowRemoteCheckBox
            // 
            this.ShowRemoteCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowRemoteCheckBox.AutoSize = true;
            this.ShowRemoteCheckBox.Checked = true;
            this.ShowRemoteCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowRemoteCheckBox.Location = new System.Drawing.Point(309, 587);
            this.ShowRemoteCheckBox.Name = "ShowRemoteCheckBox";
            this.ShowRemoteCheckBox.Size = new System.Drawing.Size(136, 24);
            this.ShowRemoteCheckBox.TabIndex = 1007;
            this.ShowRemoteCheckBox.Text = "Show Remote";
            this.ShowRemoteCheckBox.UseVisualStyleBackColor = true;
            this.ShowRemoteCheckBox.CheckedChanged += new System.EventHandler(this.ReloadDataEventHandler);
            // 
            // ShowLocalCheckBox
            // 
            this.ShowLocalCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowLocalCheckBox.AutoSize = true;
            this.ShowLocalCheckBox.Checked = true;
            this.ShowLocalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowLocalCheckBox.Location = new System.Drawing.Point(309, 626);
            this.ShowLocalCheckBox.Name = "ShowLocalCheckBox";
            this.ShowLocalCheckBox.Size = new System.Drawing.Size(117, 24);
            this.ShowLocalCheckBox.TabIndex = 1008;
            this.ShowLocalCheckBox.Text = "Show Local";
            this.ShowLocalCheckBox.UseVisualStyleBackColor = true;
            this.ShowLocalCheckBox.CheckedChanged += new System.EventHandler(this.ReloadDataEventHandler);
            // 
            // OpenFolderButton
            // 
            this.OpenFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenFolderButton.BackColor = System.Drawing.SystemColors.Control;
            this.OpenFolderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.OpenFolderButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.OpenFolderButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OpenFolderButton.Location = new System.Drawing.Point(925, 623);
            this.OpenFolderButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.OpenFolderButton.Name = "OpenFolderButton";
            this.OpenFolderButton.Size = new System.Drawing.Size(138, 37);
            this.OpenFolderButton.TabIndex = 1009;
            this.OpenFolderButton.Text = "&Open Folder";
            this.OpenFolderButton.UseVisualStyleBackColor = true;
            this.OpenFolderButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // LaunchButton
            // 
            this.LaunchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LaunchButton.BackColor = System.Drawing.SystemColors.Control;
            this.LaunchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.LaunchButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(33)))));
            this.LaunchButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LaunchButton.Location = new System.Drawing.Point(925, 576);
            this.LaunchButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LaunchButton.Name = "LaunchButton";
            this.LaunchButton.Size = new System.Drawing.Size(138, 37);
            this.LaunchButton.TabIndex = 1010;
            this.LaunchButton.Text = "&Open Wizard";
            this.LaunchButton.UseVisualStyleBackColor = true;
            this.LaunchButton.Click += new System.EventHandler(this.Control_Click);
            // 
            // AcumaticaLinkLabel
            // 
            this.AcumaticaLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AcumaticaLinkLabel.AutoSize = true;
            this.AcumaticaLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.AcumaticaLinkLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AcumaticaLinkLabel.Font = new System.Drawing.Font("Arial", 8.25F);
            this.AcumaticaLinkLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AcumaticaLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.AcumaticaLinkLabel.Location = new System.Drawing.Point(17, 664);
            this.AcumaticaLinkLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AcumaticaLinkLabel.Name = "AcumaticaLinkLabel";
            this.AcumaticaLinkLabel.Size = new System.Drawing.Size(198, 19);
            this.AcumaticaLinkLabel.TabIndex = 1001;
            this.AcumaticaLinkLabel.TabStop = true;
            this.AcumaticaLinkLabel.Text = "http://www.acumatica.com";
            this.AcumaticaLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AcumaticaLinkLabel_LinkClicked);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1076, 692);
            this.Controls.Add(this.LaunchButton);
            this.Controls.Add(this.OpenFolderButton);
            this.Controls.Add(this.ShowLocalCheckBox);
            this.Controls.Add(this.ShowRemoteCheckBox);
            this.Controls.Add(this.UninstallButton);
            this.Controls.Add(this.DownloadButton);
            this.Controls.Add(this.InstallButton);
            this.Controls.Add(this.VersionMaskedTextBox);
            this.Controls.Add(this.AcumaticaLinkLabel);
            this.Controls.Add(this.ReloadButton);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BuildPackageDataGridView);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Acumatica ERP Workspace Manager";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BuildPackageDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuildPackageBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView BuildPackageDataGridView;
        private System.Windows.Forms.BindingSource BuildPackageBindingSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button ReloadButton;
        private System.Windows.Forms.MaskedTextBox VersionMaskedTextBox;
        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.Button DownloadButton;
        private System.Windows.Forms.Button UninstallButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn majorVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn minorVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isRemoteDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isLocalDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyDataGridViewTextBoxColumn;
        private System.Windows.Forms.CheckBox ShowRemoteCheckBox;
        private System.Windows.Forms.CheckBox ShowLocalCheckBox;
        private System.Windows.Forms.Button OpenFolderButton;
        private System.Windows.Forms.Button LaunchButton;
        private System.Windows.Forms.LinkLabel AcumaticaLinkLabel;
    }
}

