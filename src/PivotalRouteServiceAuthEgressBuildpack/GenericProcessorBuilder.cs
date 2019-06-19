using System;
using System.IO;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class GenericProcessorBuilder
    {
        private readonly string buildPath;
        const string AZURE_DEVOPS_COLL_URL_ENV_VAR_NM = "AZURE_DEVOPS_COLLECTION_URL";
        const string AZURE_DEVOPS_PROJECT_NM_ENV_VAR_NM = "AZURE_DEVOPS_PROJECT_NAME";
        const string AZURE_DEVOPS_REPO_NM_ENV_VAR_NM = "AZURE_DEVOPS_REPO_NAME";
        const string AZURE_DEVOPS_ACCESS_TOKEN_ENV_VAR_NM = "AZURE_DEVOPS_ACCESS_TOKEN";
        const string AZURE_DEVOPS_SRC_KEYTAB_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM = "AZURE_DEVOPS_SOURCE_KEYTAB_FILE_URL_RELATIVE_TO_THE_ROOT";
        const string AZURE_DEVOPS_SRC_KERBEROS_CONFIG_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM = "AZURE_DEVOPS_SOURCE_KERBEROS_CONFIG_FILE_URL_RELATIVE_TO_THE_ROOT";
        const string GITHUB_KEYTAB_FILE_RAW_URL_ENV_VAR_NM = "GITHUB_KEYTAB_FILE_RAW_URL";
        const string GITHUB_KERBEROS_CONFIG_FILE_RAW_URL_ENV_VAR_NM = "GITHUB_KERBEROS_CONFIG_FILE_RAW_URL";
        const string GITHUB_ACCESS_TOKEN_ENV_VAR_NM = "GITHUB_ACCESS_TOKEN";

        public GenericProcessorBuilder(string buildPath)
        {
            this.buildPath = buildPath;
        }

        public IProcessor Build()
        {
            string kerberosConfigFilePath = Path.Combine(buildPath, "bin", "krb5.ini");

            var gitHubRawKeyTabFileUrl = Environment.GetEnvironmentVariable(GITHUB_KEYTAB_FILE_RAW_URL_ENV_VAR_NM);
            var gitHubRawkerberosConfigFileUrl = Environment.GetEnvironmentVariable(GITHUB_KERBEROS_CONFIG_FILE_RAW_URL_ENV_VAR_NM);

            var gitHubAccessToken = Environment.GetEnvironmentVariable(GITHUB_ACCESS_TOKEN_ENV_VAR_NM);
            var azureDevOpsCollectionUrl = Environment.GetEnvironmentVariable(AZURE_DEVOPS_COLL_URL_ENV_VAR_NM);
            var azureDevOpsProjectName = Environment.GetEnvironmentVariable(AZURE_DEVOPS_PROJECT_NM_ENV_VAR_NM);
            var azureDevOpsRepoName = Environment.GetEnvironmentVariable(AZURE_DEVOPS_REPO_NM_ENV_VAR_NM);
            var azureDevOpsAccessTokenl = Environment.GetEnvironmentVariable(AZURE_DEVOPS_ACCESS_TOKEN_ENV_VAR_NM);
            var azureDevOpsKerberosConfigFilePath = Environment.GetEnvironmentVariable(AZURE_DEVOPS_SRC_KERBEROS_CONFIG_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM);
            var azureDevOpsKeytabFilePath = Environment.GetEnvironmentVariable(AZURE_DEVOPS_SRC_KEYTAB_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM);


            if (!string.IsNullOrWhiteSpace(gitHubRawKeyTabFileUrl) && !string.IsNullOrWhiteSpace(gitHubRawkerberosConfigFileUrl))
            {
                return new GenericProcessor(
                    new KerberosConfigFileMover(kerberosConfigFilePath, new GitHubFileDownloader(gitHubRawkerberosConfigFileUrl, gitHubAccessToken)),
                    new KerberosConfigUpdater(kerberosConfigFilePath),
                    new KeyTabFileMover(buildPath, new GitHubFileDownloader(gitHubRawKeyTabFileUrl, gitHubAccessToken)));
            }
            else if (!string.IsNullOrWhiteSpace(azureDevOpsCollectionUrl))
            {
                ValidateRequiredVariables(azureDevOpsProjectName, azureDevOpsRepoName, azureDevOpsAccessTokenl,
                                            azureDevOpsKerberosConfigFilePath, azureDevOpsKeytabFilePath);

                return new GenericProcessor(
                                        new KerberosConfigFileMover(kerberosConfigFilePath,
                                            new AzureDevopsGitFileDownloader(azureDevOpsCollectionUrl,
                                                        azureDevOpsProjectName,
                                                        azureDevOpsRepoName,
                                                        azureDevOpsAccessTokenl,
                                                        azureDevOpsKerberosConfigFilePath)),
                                        new KerberosConfigUpdater(kerberosConfigFilePath),
                                        new KeyTabFileMover(buildPath,
                                            new AzureDevopsGitFileDownloader(azureDevOpsCollectionUrl,
                                                        azureDevOpsProjectName,
                                                        azureDevOpsRepoName,
                                                        azureDevOpsAccessTokenl,
                                                        azureDevOpsKeytabFilePath)));
            }
            else
            {
                throw new Exception("Kerberos configuration or Keytab file information not provided! Please refer to the readme for environment variable details to set them.");
            }
        }

        private static void ValidateRequiredVariables(string azureDevOpsProjectName, string azureDevOpsRepoName,
                                                        string azureDevOpsAccessTokenl, string azureDevOpsKerberosConfigFilePath,
                                                        string azureDevOpsKeytabFilePath)
        {
            if (string.IsNullOrWhiteSpace(azureDevOpsProjectName))
                throw new Exception($"{AZURE_DEVOPS_PROJECT_NM_ENV_VAR_NM} environemt variable is not set!");

            if (string.IsNullOrWhiteSpace(azureDevOpsRepoName))
                throw new Exception($"{AZURE_DEVOPS_REPO_NM_ENV_VAR_NM} environemt variable is not set!");

            if (string.IsNullOrWhiteSpace(azureDevOpsAccessTokenl))
                throw new Exception($"{AZURE_DEVOPS_ACCESS_TOKEN_ENV_VAR_NM} environemt variable is not set!");

            if (string.IsNullOrWhiteSpace(azureDevOpsKerberosConfigFilePath))
                throw new Exception($"{AZURE_DEVOPS_SRC_KERBEROS_CONFIG_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM} environemt variable is not set!");

            if (string.IsNullOrWhiteSpace(azureDevOpsKeytabFilePath))
                throw new Exception($"{AZURE_DEVOPS_SRC_KEYTAB_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM} environemt variable is not set!");
        }
    }
}
