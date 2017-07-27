using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.WorkspaceManager.Common
{
    public static class Delegates
    {
        delegate void ShowProgress(int percentDone, string progressText);
    }
}
