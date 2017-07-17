using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.UI
{
	public abstract class MessageDialog
	{
		private static MessageDialog Dialog
		{
			get
			{
				return (MessageDialog)new WindowDialog();
			}
		}

		public static DialogButtons Show(String message, ErrorLevel level, DialogButtons buttons = DialogButtons.OK)
		{
			return Show(new MessageDialogParam(message, null, level, buttons: buttons));
		}
		public static DialogButtons Show(Exception ex, ErrorLevel level, DialogButtons buttons = DialogButtons.OK)
		{
			return Show(new MessageDialogParam(null, ex, level, buttons: buttons));
		}
		public static DialogButtons Show(String message, Exception ex, ErrorLevel level, DialogButtons buttons = DialogButtons.OK)
		{
			return Show(new MessageDialogParam(message, ex, level, buttons:buttons));
		}
	
		public static DialogButtons Show(MessageDialogParam param)
		{
			DialogButtons result = DialogButtons.None;

			Boolean IsPXWaitShown = PXWait.IsShown;
			if (IsPXWaitShown) PXWait.HideWait();

			BaseDialogButton dialogResult = Dialog.InternalShow(param, BaseDialogButton.GetButtons(param.Buttons));
			if (dialogResult != null) result = dialogResult.Code;

			if (IsPXWaitShown) PXWait.ShowWait();

			return result;
		}
		protected abstract BaseDialogButton InternalShow(MessageDialogParam param, IEnumerable<BaseDialogButton> buttons);

		public static String GetMessages(Exception ex)
		{
			if (ex == null) return null;

			StringBuilder res = new StringBuilder();
			res.AppendLine(ex.Message);

			String inner = GetMessages(1, ex.InnerException);
			if (!String.IsNullOrEmpty(inner)) res.AppendLine(inner);

			return res.ToString();
		}
		protected static String GetMessages(Int32 level, Exception ex)
		{
			if (ex == null) return null;
			if (String.IsNullOrEmpty(ex.Message)) return GetMessages(level + 1, ex.InnerException);

			StringBuilder res = new StringBuilder();
			res.AppendLine(new String(' ', level) + Messages.SeparatorInner);
			res.AppendLine(new String(' ', level) + Messages.ExceptionInner);

			using(System.IO.StringReader reader = new System.IO.StringReader(ex.Message))
			{
				String s = null;
				while ((s = reader.ReadLine()) != null)
				{
					res.AppendLine(new String(' ', level) + s);
				}
			}			

			String inner = GetMessages(level + 1, ex.InnerException);
			if (!String.IsNullOrEmpty(inner)) res.AppendLine(inner);

			return res.ToString();
		}
		public static String GetStack(Exception ex)
		{
			if (ex == null) return null;			

			StringBuilder res = new StringBuilder();
			res.AppendLine(ex.StackTrace);

			String inner = GetStack(1, ex.InnerException);
			if (!String.IsNullOrEmpty(inner)) res.AppendLine(inner);

			return res.ToString();
		}
		protected static String GetStack(Int32 level, Exception ex)
		{
			if (ex == null) return null;
			if (String.IsNullOrEmpty(ex.StackTrace)) return GetStack(level + 1, ex.InnerException);

			StringBuilder res = new StringBuilder();
			res.AppendLine(new String(' ', level) + Messages.SeparatorInner);
			res.AppendLine(new String(' ', level) + Messages.StackTraceInner);

			using (System.IO.StringReader reader = new System.IO.StringReader(ex.StackTrace))
			{
				String s = null;
				while ((s = reader.ReadLine()) != null)
				{
					res.AppendLine(new String(' ', level) + s);
				}
			}	

			String inner = GetStack(level + 1, ex.InnerException);
			if (!String.IsNullOrEmpty(inner)) res.AppendLine(inner);

			return res.ToString();
		}
	}

}
