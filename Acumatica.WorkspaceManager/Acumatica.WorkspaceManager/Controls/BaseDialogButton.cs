using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigCore.UI
{

	[FlagsAttribute]
	public enum DialogButtons
	{
		None = 0,
		All = 1,
		Yes = 2,
		No = 4,
		OK = 8,
		Cancel = 16,
		Abort = 32,
		Retry = 64,
		Ignore = 128,
	}

	public abstract class BaseDialogButton
	{
		private static readonly List<BaseDialogButton> _buttons;
		internal static IEnumerable<BaseDialogButton> Buttons
		{
			get
			{
				return _buttons;
			}
		}

		public String DisplayName {get; protected set; }
		public String Shotcut { get; protected set; }
		public DialogButtons Code { get; protected set; }
		public Int32 Order { get; protected set; }
		public Boolean Default { get; protected set; }

		static BaseDialogButton()
		{
			_buttons = ReflectionHelper.GetChildTypes(typeof(BaseDialogButton)).Select(ReflectionHelper.CreateInstance<BaseDialogButton>).ToList();
			_buttons.Sort((b1, b2) => ((Int32) b1.Code) - ((Int32) b2.Code));
		}
		protected BaseDialogButton(DialogButtons code) : this(code, (Int32) code) { }
		protected BaseDialogButton(DialogButtons code, Int32 order) : this(code, code.ToString(), code.ToString().Substring(0,1), order) { }
		protected BaseDialogButton(DialogButtons code, String displayName, String shotcut, Int32 order)
		{
			DisplayName = displayName;
			Shotcut = shotcut;
			Code = code;
			Default = order < 0;
			Order = Math.Abs(order);
		}

		public static List<BaseDialogButton> GetButtons(DialogButtons buttons)
		{
			List<BaseDialogButton> res = new List<BaseDialogButton>();

			foreach (BaseDialogButton b in _buttons)
			{
				if (((buttons & b.Code) == b.Code) && !res.Contains(b)) res.Add(b);
			}

			return res;
		}
		public static BaseDialogButton GetButton(DialogButtons buttons)
		{
			return _buttons.FirstOrDefault(b => buttons == b.Code);
		}

		public override bool Equals(object obj)
		{
			if(obj.GetType() != this.GetType()) return false;
			return this.DisplayName == (obj as BaseDialogButton).DisplayName;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}


	internal class DialogButtonOk : BaseDialogButton {
		protected DialogButtonOk() : base(DialogButtons.OK, (-(Int32)DialogButtons.OK)) { }
	}
	internal class DialogButtonCancel : BaseDialogButton {
		protected DialogButtonCancel() : base(DialogButtons.Cancel) { }
	}
	internal class DialogButtonYes : BaseDialogButton {
		protected DialogButtonYes() : base(DialogButtons.Yes, (-(Int32)DialogButtons.Yes)) { }
	}
	internal class DialogButtonNo : BaseDialogButton {
		protected DialogButtonNo() : base(DialogButtons.No) { }
	}
	internal class DialogButtonAdort : BaseDialogButton {
		protected DialogButtonAdort() : base(DialogButtons.Abort) { }
	}
	internal class DialogButtonRetry : BaseDialogButton {
		protected DialogButtonRetry() : base(DialogButtons.Retry) { }
	}
	internal class DialogButtonIgnore : BaseDialogButton {
		protected DialogButtonIgnore() : base(DialogButtons.Ignore, (-(Int32)DialogButtons.Ignore)) { }
	}
	internal class DialogButtonAll : BaseDialogButton {
		protected DialogButtonAll() : base(DialogButtons.All, "Yes to All", "L", (-(Int32)DialogButtons.All)) { }
	}
}