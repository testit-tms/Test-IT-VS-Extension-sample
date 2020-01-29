using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestIT.Linker.Attributes;
using TestIT.Linker.LinkerApi;
using TestIT.Linker.Models;
using TestIT.Linker.Models.Struct;
using TestIT.Linker.Interfaces;

namespace TestIT.Linker
{
    /// <summary>
    /// Class, executing autotests creation, update and linking. Required to be created in concole app
    /// </summary>
    public class LinkExecutor
    {
        private LinkerApiFacade apiFacade;
        public static ILogger Logger;
        private ModelsRepository modelsRepository;
        List<TestToAutotestConnection> connections;

        /// <summary>
        /// Execute atotests creating, update and linking
        /// </summary>
        /// <typeparam name="TTestClassAttribute">Base type of Autotest-containibg class, implementing IAutotest interface</typeparam>
        /// <typeparam name="TTestMethodAttribute">Test method attribute</typeparam>
        /// <returns></returns>
        public async Task Execute<TTestClassAttribute, TTestMethodAttribute>(string domain, string secretKey,
            string projectNameInTestIT, string repositoryLink, string assemblyPath, ILogger logger)
            where TTestClassAttribute : Attribute
            where TTestMethodAttribute : Attribute
        {
            Logger = logger;
            Logger.Write("Executor started");
            apiFacade = new LinkerApiFacade(new Uri(domain), "PrivateToken " + secretKey);
            modelsRepository = new ModelsRepository(apiFacade);
            await modelsRepository.Initialize(projectNameInTestIT);
            var assembly = Assembly.LoadFrom(assemblyPath);
            await Initialize<TTestClassAttribute, TTestMethodAttribute>(assembly, repositoryLink);
            await ExecuteCreatingAndUpdate();
            await ExecuteLinking();
        }

        private async Task Initialize<TTestClassAttribute, TTestMethodAttribute>(Assembly assembly, string repositoryLink)
            where TTestClassAttribute : Attribute
            where TTestMethodAttribute : Attribute
        {
            modelsRepository.SetTestMethods(GetAutotestsFromAssembly<TTestClassAttribute, TTestMethodAttribute>(assembly));

            IList<AutotestModel> assemblyTestsModels = new MethodToAutotestModelConverter()
                .ConvertAll(modelsRepository.TestMethods, modelsRepository.TargetProject.Id, repositoryLink);

            modelsRepository.SetAssemblyAutotests(assemblyTestsModels);

            connections = GetTestsToAutotestConnections(modelsRepository.TestMethods);
        }

        private async Task ExecuteCreatingAndUpdate()
        {
            int succesfullyCreatedtests = 0;
            int failedToCreateTests = 0;
            int sucessfullyUpdatedTests = 0;
            int failedToUpdateTests = 0;

            Logger.Write($"{Environment.NewLine}Iniitializing Autotests Creating and updating{Environment.NewLine}");

            foreach (AutotestModel currentAssemblyAutotest in modelsRepository.AssemblyAutotests)
            {
                AutotestModel exisitingAutotest = modelsRepository
                        .ProjectAutotests
                        .FirstOrDefault(a => a.ExternalId == currentAssemblyAutotest.ExternalId);
                if (exisitingAutotest == null)
                {
                    try
                    {
                        await CreateAutotest(currentAssemblyAutotest);
                        succesfullyCreatedtests++;
                    }
                    catch (Exception ex) when (ex.Message.Contains("was not created"))
                    {
                        failedToCreateTests++;
                    }
                }
                else
                {
                    try
                    {
                        exisitingAutotest.Classname = currentAssemblyAutotest.Classname;
                        exisitingAutotest.Namespace = currentAssemblyAutotest.Namespace;

                        await apiFacade.UpdateAutotest(exisitingAutotest);
                        sucessfullyUpdatedTests++;
                    }
                    catch (Exception ex) when (ex.Message.Contains("Failed to update autotest"))
                    {
                        failedToUpdateTests++;
                        Logger.Write(ex.Message);
                    }
                }
            }
            Logger.Write($"{Environment.NewLine}Autotests Creating and updating Finished {Environment.NewLine}" +
                $"Created:{Environment.NewLine}   Successfully:'{succesfullyCreatedtests}'" +
                $"{Environment.NewLine}   Failed:'{failedToCreateTests}'{Environment.NewLine}Updated:{Environment.NewLine}   " +
                $"Successfully:'{sucessfullyUpdatedTests}'" +
                $"{Environment.NewLine}   Failed:'{failedToUpdateTests}'");
        }

        private async Task ExecuteLinking()
        {
            int successfullyLinkedTests = 0;
            int failedToLinkTests = 0;
            Logger.Write($"{Environment.NewLine}Iniitializing Autotests Linking{Environment.NewLine}");
            await modelsRepository.SetProjectAutotests();

            foreach (TestToAutotestConnection connection in connections)
            {
                WorkItemModel testCase = modelsRepository
                    .TestCases
                    .FirstOrDefault(test => test.GlobalId == connection.TestCaseGlobalId);

                if (testCase != null)
                {
                    string[] connectedAutotests = await apiFacade.GetAllAutotestNamesofTestCase(testCase);

                    if (!connectedAutotests.Contains(connection.AutotestExternalId))
                    {
                        Guid autotestId = modelsRepository
                            .ProjectAutotests
                            .First(test => test.ExternalId == connection.AutotestExternalId)
                            .Id;
                        try
                        {
                            await apiFacade.LinkAutotestToTest(autotestId, testCase.Id);
                            successfullyLinkedTests++;
                        }
                        catch (Exception ex) when (ex.Message.Contains("Failed to link autotest"))
                        {
                            failedToLinkTests++;
                            Logger.Write(ex.Message);
                        }
                    }
                }
            }

            Logger.Write($"Autotests Linking Finished" +
               $"{Environment.NewLine}   Successfully:'{successfullyLinkedTests}" +
               $"{Environment.NewLine}   Failed:'{failedToLinkTests}'");
        }

        private IList<MethodInfo> GetAutotestsFromAssembly<TTestClassAttribute, TTestMethodAttribute>(Assembly assembly)
            where TTestClassAttribute : Attribute
            where TTestMethodAttribute : Attribute
        {
            return assembly.GetTypes()
                .Where(c => c.IsDefined(typeof(TTestClassAttribute)))
                .SelectMany(t => t.GetMethods())
                .Where(m => m.IsDefined(typeof(TTestMethodAttribute)))
                .ToArray();
        }

        private async Task CreateAutotest(AutotestModel currentAssemblyAutotest)
        {
            Logger.Write($"New Autotest found: '{currentAssemblyAutotest.Name}'");
            await apiFacade.CreateAutotest(currentAssemblyAutotest);
        }

        private List<TestToAutotestConnection> GetTestsToAutotestConnections(IList<MethodInfo> fullTestMethodsList)
        {
            List<TestToAutotestConnection> connections = new List<TestToAutotestConnection>();
            foreach (MethodInfo method in fullTestMethodsList)
            {
                long[] testCaseIds = (method
                        .GetCustomAttribute(typeof(TestCaseGlobalIdAttribute)) as TestCaseGlobalIdAttribute)?.GlobalIds;
                if (testCaseIds != null)
                {
                    connections.AddRange(
                        testCaseIds.Select(id => new TestToAutotestConnection(Convert.ToInt64(id), method.Name)));
                }
            }

            return connections;
        }
    }
}
