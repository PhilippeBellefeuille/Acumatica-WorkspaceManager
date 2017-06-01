using Acumatica.WorkspaceManager.Install;
using Acumatica.WorkspaceManager.Builds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            foreach (var build in Builds.BuildManager.GetBuildPackages())
                this.buildPackageBindingSource.Add(build);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var dataGridView = (DataGridView)sender;
        }

        private void buildPackageBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            var bindingSource = (BindingSource)sender;
            var row = (BuildPackage)bindingSource.Current;

            this.button1.Enabled = row.IsLocal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var currentBuildPackage = (BuildPackage)this.buildPackageBindingSource.Current;
            
            InstallManager.InstallAcumatica(currentBuildPackage, () => {
                var yolo = "yolo";
            });
        }
    }
}
