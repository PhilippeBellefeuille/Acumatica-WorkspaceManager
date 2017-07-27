using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Acumatica.WorkspaceManager
{
    public partial class WaitForm : Form
    {
        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);
        
        private Boolean AllowClose;
		public delegate void CloseDelegate();
		public CloseDelegate CloseWait;

		public WaitForm()
		{
			CloseWait = new CloseDelegate(CloseWaitForm);

			InitializeComponent();
		}

        public void SetForeground()
        {
            BringToFront();
            SetForegroundWindow(Handle.ToInt32());
        }

        public void ShowProgress(int percentDone, string progressText)
        {
            if (Spinner.Visible)
            {
                Size = new Size(Size.Width, 150);
                Spinner.Visible = false;
            }

            if (percentDone >= 0 && percentDone <= 100)
            {
                WaitProgressBar.Value = percentDone;
                WaitProgressBar.Style = ProgressBarStyle.Blocks;
            }
            else
            {
                WaitProgressBar.Style = ProgressBarStyle.Marquee;
            }

            WaitProgressLabel.Text = progressText;
        }

        private void CloseWaitForm()
		{
			AllowClose = true;
			this.TopMost = false;
			this.Hide();
			this.Close();
		}

		private void WaitForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !AllowClose;
        }
    }

    public abstract class BaseWait
	{
		public Boolean IsCreated { get; protected set; }

		public abstract void Open();
		public abstract void Close();
        public abstract void SetForeground();
        public abstract void ShowProgress(int percentDone, string progressText);
	}
    
	public class WaitWindow : BaseWait
	{
		private WaitForm form;
		private Thread thread;
        private bool isClosing;

		public WaitWindow()
		{
			form = new WaitForm();

			thread = new Thread(threadStartPoint);
			thread.Name = "WaitFormThread";
		}
        
        public override void Open()
		{
            isClosing = false;
            thread.Start();

			while (!form.IsHandleCreated)
			{
				Thread.Sleep(50);
			}
        }
		public override void Close()
		{
            isClosing = true;

            form.Invoke(form.CloseWait);
			thread.Join();
		}

        public override void SetForeground()
        {
            if (form != null && form.IsHandleCreated && !form.IsDisposed && !isClosing)
            {
                if (form.InvokeRequired)
                {
                    form.Invoke((MethodInvoker)delegate
                    {
                        form.SetForeground();
                    });
                }
                else
                {
                    form.SetForeground();
                }
            }
        }

        public override void ShowProgress(int percentDone, string progressText)
        {
            if (form != null && form.IsHandleCreated && !form.IsDisposed && !isClosing)
            {
                if (form.InvokeRequired)
                {
                    form.Invoke((MethodInvoker)delegate
                    {
                        form.ShowProgress(percentDone, progressText);
                    });
                }
                else
                {
                    form.ShowProgress(percentDone, progressText);
                }
            }
        }

        private void threadStartPoint()
		{
			form.ShowDialog();
		}
	}
    
    public static class PXWait
	{
		public static Boolean IsStarted{ get; private set; }
		public static Boolean IsShown { get; private set; }

		public static Form CallingForm { get; private set; }
		public static BaseWait Process { get; private set; }

		public static void StartWait()
		{
			StartWait(null);
		}
		public static void StartWait(Form callingForm)
		{
			if (IsShown) return;
			if (!IsStarted) CallingForm = callingForm;
			IsStarted = true;
			ShowWait();
		}
		public static void StopWait()
		{
			if (!IsStarted) return;
			if (IsShown) HideWait();
			CallingForm = null;
			IsStarted = false;
		}

        public static void SetForeground()
        {
            if (Process != null)
            {
                Process.SetForeground();
            }
        }

        public static void ShowProgress(int percentDone, string progressText)
        {
            if (Process != null)
            {
                Process.ShowProgress(percentDone, progressText);
            }
        }
        public static void ShowWait()
		{
			if (IsShown || !IsStarted) return;
			DisableForm();
			Start();
			IsShown = true;
		}
		public static void HideWait()
		{
			if (!IsShown) return;
			EnableForm();
			Stop();
			IsShown = false;
		}

		private static void Start()
		{
            Process = new WaitWindow();
			Process.Open();
        }
		private static void Stop()
		{
			Process.Close();
		}

		private static void DisableForm()
		{
			if(CallingForm != null) CallingForm.Enabled = false;
			Cursor.Current = Cursors.WaitCursor;
		}
		private static void EnableForm()
		{
			if (CallingForm != null) CallingForm.Enabled = true;
			Cursor.Current = Cursors.Arrow;
		}
	}

    
}
