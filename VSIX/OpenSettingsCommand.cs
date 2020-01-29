using System;
using System.ComponentModel.Design;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using VSIXProject3.Services;
using Task = System.Threading.Tasks.Task;

namespace VSIXProject3
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class OpenSettingsCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0102;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f975cca0-b2fb-4359-bf36-d5e05d91988a");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenSettingsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private OpenSettingsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        //This field will contain ex of DTE interface
        private static DTE ActiveVS;


        /// <summary>
        /// Get current DTE of active solution using IServiceProvider
        /// </summary>
        /// <param name="provider">Global provider</param>
        /// <returns>Ex of the current DTE</returns>
        private static DTE GetCurrentDTE(IServiceProvider provider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ActiveVS = (DTE)provider.GetService(typeof(DTE));
            if (ActiveVS == null) throw new InvalidOperationException("DTE not found.");
            return ActiveVS;
        }

        /// <summary>
        /// Get current DTE of active solution using default IServiceProvider
        /// </summary>
        /// <returns>Ex of the current DTE</returns>
        private static DTE GetCurrentDTE()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetCurrentDTE(Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OpenSettingsCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in OpenSettingsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new OpenSettingsCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                DteService dteService = new DteService();
                SettingsWindow settingsWindow = new SettingsWindow(dteService.GetProjectsInSolution(GetCurrentDTE()));
                settingsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some problems with load settings window. {Environment.NewLine} Exception message: {ex.Message}");
            }
            
        }
    }
}
