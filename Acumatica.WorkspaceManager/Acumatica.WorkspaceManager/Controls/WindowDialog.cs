using System;
using System.Collections.Generic;

namespace ConfigCore.UI
{
	internal class WindowDialog : MessageDialog
	{
		protected override BaseDialogButton InternalShow(MessageDialogParam param, IEnumerable<BaseDialogButton> buttons)
		{
			FormMessage form = new FormMessage(param, buttons);
			if (param.Owner != null && param.Owner.InvokeRequired)
				param.Owner.Invoke(new Action(() => form.ShowDialog(param.Owner)));
			else
				form.ShowDialog(param.Owner);
			return form.Result;
		}
	}
}