using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows;

namespace Kruchy.Plugin.UI
{
    public class WindowTools
    {
        public static void ShowWindow(object window)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ToolWindowPane windowPane = (ToolWindowPane)window;
            if ((null == windowPane) || (null == windowPane.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)windowPane.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public static void ShowWindowModal(
            object window,
            IVsUIShell uiShell)
        {
            var objectWindow = (Window)window;
            
            ThreadHelper.ThrowIfNotOnUIThread();

            IntPtr hwnd;
            uiShell.GetDialogOwnerHwnd(out hwnd);
            uiShell.EnableModeless(0);
            try
            {
                WindowHelper.ShowModal(objectWindow, hwnd);
            }
            finally
            {
                // This will take place after the window is closed.
                uiShell.EnableModeless(1);
            }
        }
    }
}
