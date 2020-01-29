using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Task = System.Threading.Tasks.Task;
using VSIXProject3.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.IO;
using TestIT.Linker;

namespace VSIXProject3
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SendPushCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f975cca0-b2fb-4359-bf36-d5e05d91988a");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private static PaneLogService Logger;

        private static DTE ActiveVS;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPushCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SendPushCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            Logger = new PaneLogService();
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        private static DTE GetCurrentDTE(IServiceProvider provider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ActiveVS = (DTE)provider.GetService(typeof(DTE));
            if (ActiveVS == null)
                throw new InvalidOperationException("Активной DTE не найдено.");
            Logger.Write("Добро пожаловать в Test IT System!");
            return ActiveVS;
        }

        private static DTE GetCurrentDTE()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetCurrentDTE(Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider);
        }


        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SendPushCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider
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
            // Switch to the main thread - the call to AddCommand in SendPushCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SendPushCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Logger.Write("Test IT работает в основном потоке.");
            var dte = GetCurrentDTE();

            //Create list of projects in active solution
            var activeProjects = new List<Project>();
            activeProjects.AddRange(dte.Solution.Projects.Cast<Project>());
            if (activeProjects.Count == 0)
            {
                Logger.Write("Ошибка: решение не содержит ни одного активного проекта.");
                return;
            }

            //Call link.api and try to execute using properties
            var settings = Properties.Settings.Default;
            var executor = new LinkExecutor();
            try
            {
                //Call library
                await executor.Execute<TestClassAttribute, TestMethodAttribute>(
                    settings.Domain,
                    settings.SecretKey,
                    settings.ProjectNameInTestIT,
                    settings.RepositoryLink ?? string.Empty,
                    settings.AssemblyPath,
                    Logger);
            }
            catch (FileNotFoundException ex)
            {
                Logger.Write($"Пересоберите проект и попробуйте ещё раз.{Environment.NewLine}" +
                    $"Сообщение исключения: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Write($"Что-то пошло не так. Проверьте параметры и попробуйте ещё раз.{Environment.NewLine}" +
                    $"Сообщение исключения: {ex.Message}");
            }
            finally
            {
                System.Windows.MessageBox.Show("Расширение закончило работу. Подробности смотрите в Test IT Log.");
            }
        }
    }
}
