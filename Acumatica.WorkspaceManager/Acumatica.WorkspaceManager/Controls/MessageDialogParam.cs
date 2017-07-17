using System;
using System.Windows.Forms;

namespace ConfigCore.UI
{
	public class MessageDialogParam
	{
		public String Message { get; set; }
		public Exception Error { get; set; }
		public ErrorLevel Level { get; set; }
		public Boolean Critical { get; set; }
		public DialogButtons Buttons { get; set; }
		public String Script { get; set; }
		public Form Owner;

		public MessageDialogParam(String message, Exception ex, ErrorLevel level, Boolean critical = false, DialogButtons buttons = DialogButtons.OK)
		{
			if (String.IsNullOrEmpty(message) && ex == null) 
				throw new ArgumentNullException("message");

			Message = message ?? ex.Message;
			Error = ex;
			Level = level;
			Critical = critical;
			Buttons = buttons;
		}
	}
}