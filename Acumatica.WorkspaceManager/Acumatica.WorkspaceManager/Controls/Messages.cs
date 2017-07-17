using System;
using System.Collections.Generic;

namespace ConfigCore
{
    public static class Messages
    {
        public interface IDisplayParam
        {
            String GetValue();
            String GetDisplayValue();
            void SetValue(String val);

            String DisplayName { get; }
        }

        public class PXString
        {
            public String Message { get; private set; }
            public Int32 ErrorCode { get; private set; }

            public PXString(String Message)
            {
                this.Message = Message;
                ErrorCode = 0;
            }
            public PXString(String Message, Int32 Code)
            {
                this.Message = Message;
                ErrorCode = Code;
            }

            public PXString Format(params Object[] obj)
            {
                List<Object> list = new List<Object>();

                foreach (Object o in obj)
                {
                    if (o == null)
                    {
                        list.Add(String.Empty);
                        continue;
                    }

                    if (o.GetType().GetInterface("IInstParam", true) == null) list.Add(o);
                    else
                    {
                        String val = ((IDisplayParam)o).GetValue();
                        if (val == null) list.Add(String.Empty);
                        else list.Add(val);
                    }
                }

                return new PXString(String.Format(Message, list.ToArray()), ErrorCode);
            }

            public override string ToString()
            {
                return Message;
            }

            public static implicit operator String(PXString s)
            {
                return s.Message;
            }
            public static PXString Empty = new PXString(String.Empty, 0);
        }

		public static PXString SeparatorInner = new PXString("--------------------------------------");
		public static PXString SeparatorOuter = new PXString("==================={0}===================");
		public static PXString ExceptionInner = new PXString("Inner Exception:");
		public static PXString StackTraceInner = new PXString("Inner Stack:");
	}
}
