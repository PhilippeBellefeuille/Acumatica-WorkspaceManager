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
            this.tabBuild = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.majorVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.minorVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isRemoteDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.isLocalDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.keyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildPackageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabBuild.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildPackageBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tabBuild
            // 
            this.tabBuild.Controls.Add(this.tabPage1);
            this.tabBuild.Controls.Add(this.tabPage2);
            this.tabBuild.Location = new System.Drawing.Point(-1, 48);
            this.tabBuild.Name = "tabBuild";
            this.tabBuild.SelectedIndex = 0;
            this.tabBuild.Size = new System.Drawing.Size(1189, 682);
            this.tabBuild.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1181, 656);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Install";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.majorVersionDataGridViewTextBoxColumn,
            this.minorVersionDataGridViewTextBoxColumn,
            this.buildNumberDataGridViewTextBoxColumn,
            this.isRemoteDataGridViewCheckBoxColumn,
            this.isLocalDataGridViewCheckBoxColumn,
            this.keyDataGridViewTextBoxColumn});
            this.dataGridView1.DataBindings.Add(new System.Windows.Forms.Binding("Tag", this.buildPackageBindingSource, "Key", true));
            this.dataGridView1.DataSource = this.buildPackageBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(0, 54);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1181, 606);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // majorVersionDataGridViewTextBoxColumn
            // 
            this.majorVersionDataGridViewTextBoxColumn.DataPropertyName = "MajorVersion";
            this.majorVersionDataGridViewTextBoxColumn.HeaderText = "MajorVersion";
            this.majorVersionDataGridViewTextBoxColumn.Name = "majorVersionDataGridViewTextBoxColumn";
            this.majorVersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // minorVersionDataGridViewTextBoxColumn
            // 
            this.minorVersionDataGridViewTextBoxColumn.DataPropertyName = "MinorVersion";
            this.minorVersionDataGridViewTextBoxColumn.HeaderText = "MinorVersion";
            this.minorVersionDataGridViewTextBoxColumn.Name = "minorVersionDataGridViewTextBoxColumn";
            this.minorVersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // buildNumberDataGridViewTextBoxColumn
            // 
            this.buildNumberDataGridViewTextBoxColumn.DataPropertyName = "BuildNumber";
            this.buildNumberDataGridViewTextBoxColumn.HeaderText = "BuildNumber";
            this.buildNumberDataGridViewTextBoxColumn.Name = "buildNumberDataGridViewTextBoxColumn";
            this.buildNumberDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // isRemoteDataGridViewCheckBoxColumn
            // 
            this.isRemoteDataGridViewCheckBoxColumn.DataPropertyName = "IsRemote";
            this.isRemoteDataGridViewCheckBoxColumn.HeaderText = "IsRemote";
            this.isRemoteDataGridViewCheckBoxColumn.Name = "isRemoteDataGridViewCheckBoxColumn";
            this.isRemoteDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // isLocalDataGridViewCheckBoxColumn
            // 
            this.isLocalDataGridViewCheckBoxColumn.DataPropertyName = "IsLocal";
            this.isLocalDataGridViewCheckBoxColumn.HeaderText = "IsLocal";
            this.isLocalDataGridViewCheckBoxColumn.Name = "isLocalDataGridViewCheckBoxColumn";
            this.isLocalDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // keyDataGridViewTextBoxColumn
            // 
            this.keyDataGridViewTextBoxColumn.DataPropertyName = "Key";
            this.keyDataGridViewTextBoxColumn.HeaderText = "Key";
            this.keyDataGridViewTextBoxColumn.Name = "keyDataGridViewTextBoxColumn";
            this.keyDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // buildPackageBindingSource
            // 
            this.buildPackageBindingSource.DataSource = typeof(Acumatica.WorkspaceManager.Builds.BuildPackage);
            this.buildPackageBindingSource.Filter = "MajorVersion, MinorVersion";
            this.buildPackageBindingSource.Sort = "MajorVersion, MinorVersion, BuildNumber";
            this.buildPackageBindingSource.CurrentChanged += new System.EventHandler(this.buildPackageBindingSource_CurrentChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1181, 656);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 730);
            this.Controls.Add(this.tabBuild);
            this.Name = "Main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabBuild.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildPackageBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabBuild;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn majorVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn minorVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buildNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isRemoteDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isLocalDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource buildPackageBindingSource;
        private System.Windows.Forms.Button button1;
    }
}

