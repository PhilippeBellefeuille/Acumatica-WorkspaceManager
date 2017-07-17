using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConfigCore.UI
{
	public partial class FormMessage : Form
	{
		private Int32 _Detail_Heigth = 189;

		public BaseDialogButton Result { get; private set; }
		private MessageDialogParam Param { get; set; }
		private IEnumerable<BaseDialogButton> Buttons { get; set; }

		private String InnerMessage { get; set; }
		private String InnerStack { get; set; }
		private String Script { get; set; }


		public FormMessage(MessageDialogParam parameter, IEnumerable<BaseDialogButton> buttons)
		{
			Param = parameter;
			Buttons = buttons;

			InitializeComponent();
			Initialise();
		}

		private void Initialise()
		{
			InitialiseView();
			InitialiseIcon();
			InitialiseText();
			InitialiseButtons();
			InitialiseDetails();
		}
		private void InitialiseView()
		{
			this.Height = this.Height - tbDetalis.Height;

			HideDetail();
		}
		private void InitialiseIcon()
		{
			switch (Param.Level)
			{
				case ErrorLevel.CriticalError:
				case ErrorLevel.Error:
					icon.Image = icons.Images["Error"];
					break;
				case ErrorLevel.Warning: 
				case ErrorLevel.CriticalWarning:
					icon.Image = icons.Images["Warning"];
					break;
				case ErrorLevel.Note:
				default:
					icon.Image = icons.Images["Information"];
					break;
			}
		}
		private void InitialiseText()
		{
			InnerMessage = MessageDialog.GetMessages(Param.Error);
			InnerStack = MessageDialog.GetStack(Param.Error);
			Script = Param.Script;

			tbMessage.Text = Param.Message;
		}
		private void InitialiseButtons()
		{
			Button simpleBtn = new Button();
			Int32 top = buttonsPanel.Height / 2 - simpleBtn.Height / 2;
			Int32 width = simpleBtn.Width + 15;
			Int32 start = /*buttonsPanel.Left + */(buttonsPanel.Width - width * Buttons.Count()) / 2;

			BaseDialogButton defaultBtn = null;
			for (int i = Buttons.Count() - 1; i >= 0; i--)
			{
				BaseDialogButton btn = Buttons.ElementAt(i);
				if (defaultBtn == null) defaultBtn = btn;

				if ((defaultBtn.Default && btn.Default && btn.Order < defaultBtn.Order) 
					|| (!defaultBtn.Default && btn.Order < defaultBtn.Order)
					|| (!defaultBtn.Default && !btn.Default))			
					defaultBtn = btn;
			}

			List<Button> btns = new List<Button>();
			foreach(BaseDialogButton button in Buttons)
			{
				Button btn = new Button();
				btn.Name = "btn" + button.Code.ToString();
				btn.Text = button.DisplayName;
				btn.Tag = button;
				btn.TabIndex = button.Order;
				btn.Top = top;
				btn.Visible = true;
				btn.UseVisualStyleBackColor = true;
				btn.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

				if (button == defaultBtn) this.AcceptButton = btn;
				btn.Click += new System.EventHandler(this.btnResult_Click);

				btns.Add(btn);
			}

			// set left param
			for (int i = 0; i < btns.Count; i++)
			{
				btns[i].Left = start + width * i + 1;				
			}

			buttonsPanel.Controls.AddRange(btns.ToArray());
			buttonsPanel.Refresh();
		}
		private void InitialiseDetails()
		{
			if (String.IsNullOrEmpty(Script)) btnShowScript.Visible = false;
			if (String.IsNullOrEmpty(InnerMessage)) btnShowMessage.Visible = false;
			if (String.IsNullOrEmpty(InnerStack)) btnSthowStack.Visible = false;
		}
		private void ShowDetail()
		{
			if (!tbDetalis.Visible)
			{
				//this.MinimumSize = new Size(0, 0);
				this.Height = this.Height + _Detail_Heigth;
				//this.MinimumSize = this.Size;
			}

			tbDetalis.Visible = true;
			btnHide.Visible = true;
		}
		private void HideDetail()
		{
			if (tbDetalis.Visible)
			{
				//this.MinimumSize = new Size(0, 0);
				this.Height = this.Height - tbDetalis.Height;
				//this.MinimumSize = this.Size;
			}				

			tbDetalis.Visible = false;
			btnHide.Visible = false;
		}

		private void btnHide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			HideDetail();
		}
		private void btnShowMessage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShowDetail();

			tbDetalis.Text = InnerMessage;
		}
		private void btnSthowStack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShowDetail();

			tbDetalis.Text = InnerStack;
		}
		private void btnShowScript_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShowDetail();

			tbDetalis.Text = Script;
		}

		private void btnResult_Click(object sender, EventArgs e)
		{
			BaseDialogButton btn = (sender as Button).Tag as BaseDialogButton;
			Result = btn;
			this.Close();
		}
	}
}
