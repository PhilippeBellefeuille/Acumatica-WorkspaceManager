using ConfigCore.UI;
using System;

namespace ConfigCore
{
    public interface ISysData { }
    public enum ErrorLevel
    {
        Note,
        Warning,
        CriticalWarning,
        Error,
        CriticalError
    }

    public static class SysData
    {
        public static Boolean AUTO;

        public static Boolean CanPositionCursor
        {
            get
            {
                try
                {
                    Int32 testVal = Console.CursorLeft + Console.CursorTop;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
        
        public static void ShowException(String message)
        {
            MessageDialog.Show(message, ErrorLevel.Error);
        }

        public static void ShowException(String message, ErrorLevel level)
        {
            MessageDialog.Show(message, level);
        }

        public static void ShowException(String message, ErrorLevel level, Exception ex)
        {
            MessageDialog.Show(message, ex, level);
        }
    }
}