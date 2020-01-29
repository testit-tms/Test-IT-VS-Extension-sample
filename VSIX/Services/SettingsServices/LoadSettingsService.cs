namespace VSIXProject3.Services.SettingsServices
{
    class LoadSettingsService
    {
        /// <summary>
        /// Call this method to get all parametres of application
        /// </summary>
        /// <returns>Method return all parametres from properties</returns>
        public (string Domain, string SecretKey, string ProjectNameInTestIT, string RepositoryLink, string AssemblyPath, string ProjectName) Load()
        {
            var localSettings = Properties.Settings.Default;
            return (
                localSettings.Domain,
                localSettings.SecretKey,
                localSettings.ProjectNameInTestIT,
                localSettings.RepositoryLink,
                localSettings.AssemblyPath,
                localSettings.ProjectName
                );
        }
    }
}
