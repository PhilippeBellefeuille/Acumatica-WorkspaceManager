using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Acumatica.WorkspaceManager
{
    static class Program
    {
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsUserAnAdmin();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.VisualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;

            if (IsUserAnAdmin())
            {
                Application.Run(new Main());
            }
            else
            {
                Process.Start(new ProcessStartInfo(Assembly.GetExecutingAssembly().Location)
                {
                    Verb = "runas"
                });
            }
        }
    }
}
