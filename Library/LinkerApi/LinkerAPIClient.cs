using System;
using System.Net.Http;
using System.Threading.Tasks;
using TestIT.API;
using TestIT.Linker.Models;
using TestIT.Linker.Models.Project;

namespace TestIT.Linker.LinkerApi
{
    class LinkerAPIClient
    {
        private HttpClient client = new HttpClient();
        public LinkerAPIClient(Uri domain, string token)
        {
            client.BaseAddress = domain;
            client.DefaultRequestHeaders.Add("Authorization", token);
        }

        public async Task<Response<ProjectModel[]>> GetAllProjects()
        {
            HttpResponseMessage response = await client.GetAsync("api/v2/projects?isDeleted=false");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Секретный ключ API не совпадает");
            return await Response.CreateValueResponse<ProjectModel[]>(response);
        }

        public async Task<Response<WorkItemModel[]>> GetAllTestCasesOfProject(Guid projectId)
        {
            HttpResponseMessage response = await client.GetAsync($"api/v2/projects/{projectId}/workItems" +
                "?SearchField=entityTypeName&SearchValue=TestCases");
            return await Response.CreateValueResponse<WorkItemModel[]>(response);
        }

        public async Task<Response<AutotestModel[]>> GetAllAutotestsOfProject(Guid projectId)
        {
            HttpResponseMessage response = await client.GetAsync($"api/v2/autoTests?projectId={projectId}");
            return await Response.CreateValueResponse<AutotestModel[]>(response);
        }

        public async Task<Response<AutotestModel>> CreateAutotests(AutotestModel autotest)
        {
            var item = new
            {
                externalId = autotest.ExternalId,
                linkToRepository = autotest.LinkToRepository,
                projectId = autotest.ProjectId,
                name = autotest.Name,
                @namespace = autotest.Namespace,
                classname = autotest.Classname,
                steps = new string[] { }
            };

            var response = await client.PostAsync("api/v2/autoTests", new JsonContent(item));
            return await Response.CreateValueResponse<AutotestModel>(response);
        }

        public async Task<Response<AutotestModel>> UpdateAutotest(AutotestModel autotest)
        {
            var response = await client.PutAsync($"api/v2/autoTests", new JsonContent(autotest));
            return await Response.CreateValueResponse<AutotestModel>(response);
        }

        public async Task<Response<AutotestModel[]>> GetAllAutotestsOfTestCase(WorkItemModel autotest)
        {
            HttpResponseMessage response = await client.GetAsync($"api/v2/workItems/{autotest.Id}/autoTests");
            return await Response.CreateValueResponse<AutotestModel[]>(response);
        }

        public async Task<Response> LinkAutotestToTest(Guid autoTestId, Guid testCaseId)
        {
            var response = await client.PostAsync($"api/v2/autoTests/{autoTestId}/workItems",
                new JsonContent(new TestCaseIdModel() { Id = testCaseId }));
            return await Response.CreateValueResponse<AutotestModel>(response);
        }

        public async Task<Response> DeleteAutotest(Guid autoTestId)
        {
            var response = await client.DeleteAsync($"api/v2/autoTests/{autoTestId}");
            return await Response.CreateValueResponse<AutotestModel>(response);
        }
    }
}
