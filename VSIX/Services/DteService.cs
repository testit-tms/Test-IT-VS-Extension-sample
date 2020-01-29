using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using VSIXProject3.Models;

namespace VSIXProject3.Services
{
    public class DteService
    {
        private static DTE ActiveVS;

        /// <summary>
        /// Get projects list in active solution
        /// </summary>
        /// <param name="activeVS">Active DTE ex</param>
        /// <returns>Return all project from active solution</returns>
        public List<ProjectModel> GetProjectsInSolution(DTE activeVS)
        {
            ActiveVS = activeVS;
            var projects = new List<ProjectModel>();

            var activeProjects = new List<Project>();
            activeProjects.AddRange(ActiveVS.Solution.Projects.Cast<Project>());
            foreach (var item in activeProjects)
            {
                projects.Add(new ProjectModel()
                {
                    ProjectName = item.Name,
                    ProjectPath = item.FullName
                });
            }
            return projects;
        }

        /// <summary>
        /// Get all dll-files of the current test project
        /// </summary>
        /// <param name="project">Current project in solution</param>
        /// <returns>List with all dll-files in current project</returns>
        public List<LibsInProjectModel> GetAllDLLOfProject(ProjectModel project)
        {
            var libraries = new List<LibsInProjectModel>();

            try
            {
                var pathes = Directory.GetFiles(
                    $"{Path.GetDirectoryName(project.ProjectPath)}/bin/Debug",
                    "*.dll",
                    SearchOption.AllDirectories);

                foreach (var libPath in pathes)
                {
                    libraries.Add(new LibsInProjectModel()
                    {
                        LibraryPath = libPath,
                        LibraryName = Path.GetFileName(libPath)
                    });
                }
            }
            catch(DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException("Каталог 'bin/Debug' не был обнаружен. " +
                    "Соберите проект и вызовите расширение ещё раз.");
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return libraries;
        }

        public LibsInProjectModel GetCurrentDLLOfProject(ProjectModel project, string libraryName)
        {
            try
            {
                var libraryPath = Directory.GetFiles(
                    $"{Path.GetDirectoryName(project.ProjectPath)}/bin/Debug",
                    libraryName,
                    SearchOption.AllDirectories).First();
                return new LibsInProjectModel() { LibraryName = libraryName, LibraryPath = libraryPath };
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Выбранная сборка отсутствует в каталоге 'bin/Debug'. " +
                    "Проверьте наличие сборки и повторите попытку.");
            }
        }
    }
}
