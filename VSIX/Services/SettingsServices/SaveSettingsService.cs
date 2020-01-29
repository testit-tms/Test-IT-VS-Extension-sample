using System;

namespace VSIXProject3.Services.SettingsServices
{
    class SaveSettingsService
    {
        public void Save(string domain, string secretKey, string projectNameInTestIT, string assemblyPath, string projectName, string actualSolutionPath, string repositoryLink)
        {
            var localSettings = Properties.Settings.Default;
            localSettings.Domain = domain ?? throw new NullReferenceException("Domain can not be null.");
            localSettings.SecretKey = secretKey ?? throw new NullReferenceException("Secret key can not be null. Use secret key from TestIT portal.");
            localSettings.ProjectNameInTestIT = projectNameInTestIT ?? throw new NullReferenceException("Project name can not be null. Use project name from TestIT portal.");
            localSettings.AssemblyPath = assemblyPath ?? throw new NullReferenceException("Path to assembly (*.DLL) can not be null.");
            localSettings.ProjectName = projectName ?? throw new NullReferenceException("Name of the project can not be null.");
            localSettings.ActualSolutionPath = actualSolutionPath ?? throw new NullReferenceException("Path to solution can not be null.");
            localSettings.RepositoryLink = repositoryLink ?? throw new NullReferenceException("Link to repository can not be null.");
            localSettings.Save();
        }
    }
}
