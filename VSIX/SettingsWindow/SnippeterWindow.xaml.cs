using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using VSIXProject3.Models;
using VSIXProject3.Services;
using VSIXProject3.Services.SettingsServices;

namespace VSIXProject3
{
    /// <summary>
    /// Interaction logic for DetailsDialog.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public List<ProjectModel> ProjectsInActiveSolution = new List<ProjectModel>();

        /// <summary>
        /// Creates the window and loads either the editor or the manager
        /// </summary>
        internal SettingsWindow(List<ProjectModel> projectsInActiveSolution)
        {
            this.ProjectsInActiveSolution = projectsInActiveSolution;

            LoadSettingsService loadSettingsService = new LoadSettingsService();

            var settings = (new LoadSettingsService()).Load();

            InitializeComponent();

            domain_content.Text = settings.Domain;
            secretKey_content.Text = settings.SecretKey;
            projectNameInTestIT_id_content.Text = settings.ProjectNameInTestIT;
            local_test_project_content.ItemsSource = projectsInActiveSolution.Select(x => x.ProjectName);
            local_test_project_content.SelectedIndex = projectsInActiveSolution.FindIndex(x => x.ProjectName == settings.ProjectNameInTestIT);
            local_test_project_content.SelectionChanged += Local_test_project_content_SelectionChanged;
            repositoryLink_content.Text = settings.RepositoryLink;
        }

        /// <summary>
        /// Trigger: when selection change of the local_test_project_content combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Local_test_project_content_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var dteService = new DteService();
                var libs = dteService.GetAllDLLOfProject(ProjectsInActiveSolution.FirstOrDefault(x => x.ProjectName == local_test_project_content.SelectedItem.ToString()));
                projectDll_content.ItemsSource = libs.Select(x => x.LibraryName);
            }
            catch(DirectoryNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            var dteService = new DteService();
            var settings = new SaveSettingsService();
            var project = new ProjectModel()
            {
                ProjectName = local_test_project_content.Text,
                ProjectPath = ProjectsInActiveSolution.First(x => x.ProjectName == local_test_project_content.Text).ProjectPath
            };

            try
            {
                settings.Save(
                    domain: domain_content.Text,
                    secretKey: secretKey_content.Text,
                    projectNameInTestIT: projectNameInTestIT_id_content.Text,
                    assemblyPath:  dteService.GetCurrentDLLOfProject(project, projectDll_content.Text).LibraryPath,
                    projectName: local_test_project_content.Text,
                    actualSolutionPath: ProjectsInActiveSolution.First(x => x.ProjectName == local_test_project_content.Text).ProjectPath,
                    repositoryLink: repositoryLink_content.Text
                    );
            }
            catch(NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Кажется, что-то пошло не так. Сообщение исключения: {ex.Message}");
            }

            this.Close();
        }
    }
}
