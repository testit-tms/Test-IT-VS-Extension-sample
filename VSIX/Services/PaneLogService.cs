using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using TestIT.Linker.Interfaces;

namespace VSIXProject3.Services
{
    public class PaneLogService : ILogger
    {
        private static IVsOutputWindowPane Pane { get; set; }
        public PaneLogService()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the output window
            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            // Ensure that the desired pane is visible
            var paneGuid = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            IVsOutputWindowPane pane;
            outputWindow.CreatePane(paneGuid, "Test IT Log", 1, 0);
            outputWindow.GetPane(paneGuid, out pane);
            Pane = pane;            
        }

        public void Write(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Output the message
            Pane.OutputString(message + Environment.NewLine);
        }
    }
}
