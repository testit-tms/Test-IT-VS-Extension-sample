using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TestIT.Linker.LinkerApi;
using TestIT.Linker.Models;
using TestIT.Linker.Models.Project;

namespace TestIT.Linker
{
    public class ModelsRepository
    {
        private readonly LinkerApiFacade apiFacade;
        public ModelsRepository(LinkerApiFacade apiFacade)
        {
            this.apiFacade = apiFacade;
        }

        public ProjectModel TargetProject { get; private set; }
        /// <summary>
        /// All target project Test Cases (eiher linked to autotests, or not)
        /// </summary>
        public IList<WorkItemModel> TestCases { get; private set; }
        /// <summary>
        /// All autotests, exisiting in target project
        /// </summary>
        public IList<AutotestModel> ProjectAutotests { get; private set; }
        /// <summary>
        /// All autotests, created from found sutotests methods in target assembly
        /// </summary>
        public IList<AutotestModel> AssemblyAutotests { get; private set; }
        /// <summary>
        /// All autottest methods, found in target assembly
        /// </summary>
        public IList<MethodInfo> TestMethods { get; private set; }

        public async Task Initialize(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new Exception("ProjectName is required!");
            }

            TargetProject = await apiFacade.GetProject(projectName);

            await SetTestCases();
            await SetProjectAutotests();
        }

        public void SetAssemblyAutotests(IList<AutotestModel> createdModels)
        {
            AssemblyAutotests = createdModels;
        }
        private async Task SetTestCases()
        {
            TestCases = await apiFacade.GetAllTestCasesOfProject(TargetProject);
        }

        public async Task SetProjectAutotests()
        {
            ProjectAutotests = await apiFacade.GetAllAutotestsOfProject(TargetProject);
        }

        public void SetTestMethods(IList<MethodInfo> methods)
        {
            TestMethods = methods;
        }
    }
}
