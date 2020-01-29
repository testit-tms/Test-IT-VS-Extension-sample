using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestIT.API;
using TestIT.Linker.Models;
using TestIT.Linker.Models.Project;

namespace TestIT.Linker.LinkerApi
{
    public class LinkerApiFacade
    {
        private LinkerAPIClient client;

        public LinkerApiFacade(Uri domain, string token)
        {
            client = new LinkerAPIClient(domain, token);
        }

        public async Task<ProjectModel> GetProject(string name)
        {
            ProjectModel[] allProjects = (await client.GetAllProjects()).Value;
            ProjectModel targetProject = allProjects.FirstOrDefault(project => project.Name == name);
            if (targetProject == null)
            {
                throw new Exception($"Project '{name}' not found!");
            }

            return targetProject;
        }

        public async Task<WorkItemModel[]> GetAllTestCasesOfProject(ProjectModel project)
        {
            return (await client.GetAllTestCasesOfProject(project.Id)).Value;
        }

        public async Task<AutotestModel[]> GetAllAutotestsOfProject(ProjectModel project)
        {
            return (await client.GetAllAutotestsOfProject(project.Id)).Value;
        }

        public async Task<AutotestModel> CreateAutotest(AutotestModel precreatedModel)
        {
            Response<AutotestModel> result = await client.CreateAutotests(precreatedModel);

            if (result.Code != HttpStatusCode.Created)
            {
                throw new Exception($"Autotests '{precreatedModel.Name}' was not created! " +
                    $"Response code is '{result.Code}', Content: '{result.Content}'");
            }

            return result.Value;
        }

        public async Task UpdateAutotest(AutotestModel modelWithUpdate)
        {
            Response result = await client.UpdateAutotest(modelWithUpdate);
            if (result.Code != HttpStatusCode.NoContent)
                throw new Exception($"Failed to update autotest ID = {modelWithUpdate.ExternalId}! " +
                    $"Response code is '{result.Code}', content: {result.Content}");
        }

        public async Task<string[]> GetAllAutotestNamesofTestCase(WorkItemModel testCase)
        {
            AutotestModel[] result = (await client.GetAllAutotestsOfTestCase(testCase)).Value;
            return result.Select(test => test.ExternalId).ToArray();
        }

        public async Task LinkAutotestToTest(Guid autoTestId, Guid testCaseId)
        {
            Response result = await client.LinkAutotestToTest(autoTestId, testCaseId);
            if (result.Code != HttpStatusCode.NoContent)
                throw new Exception($"Failed to link autotest ID = {autoTestId} with test ID = {testCaseId}! " +
                    $"Response code is '{result.Code}', content: {result.Content}");
        }

        public async Task DeleteAutotest(AutotestModel modelToDelete)
        {
            Response result = await client.DeleteAutotest(modelToDelete.Id);

            if (result.Code != HttpStatusCode.NoContent)
                throw new Exception($"Failed to delete '{modelToDelete.Name}' test! Response code is '{result.Code}', " +
                    $"content: {result.Content}");
        }
    }
}
