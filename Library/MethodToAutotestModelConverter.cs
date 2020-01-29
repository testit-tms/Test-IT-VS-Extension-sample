using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TestIT.Linker.Models;

namespace TestIT.Linker
{
    public class MethodToAutotestModelConverter
    {
        public IList<AutotestModel> ConvertAll(IList<MethodInfo> testMethods, Guid projectId, string repositoryLink)
        {
            List<AutotestModel> allModels = new List<AutotestModel>();

            foreach (MethodInfo method in testMethods)
                allModels.Add(Convert(method, projectId, repositoryLink));

            return allModels;
        }

        private AutotestModel Convert(MethodInfo testMethod, Guid projectId, string repositoryLink)
        {
            AutotestModel model = new AutotestModel()
            {
                ExternalId = testMethod.Name,
                LinkToRepository = repositoryLink,
                ProjectId = projectId,
                Name = GetAutotestName(testMethod.Name),
                Classname = testMethod.DeclaringType.Name,
                Namespace = GetAutotestNamespace(testMethod)
            };

            return model;
        }

        private string GetAutotestName(string autotestExternalId)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < autotestExternalId.Length; i++)
            {
                if(char.IsUpper(autotestExternalId[i]) &&  i != 0)
                    stringBuilder.Append(" ");
                stringBuilder.Append(autotestExternalId[i]);
            }

            return stringBuilder.ToString();
        }

        private string GetAutotestNamespace(MethodInfo testMethod)
        {
            return testMethod.DeclaringType.FullName
                .Replace($".{testMethod.DeclaringType.Name}", string.Empty);
        }
    }
}
