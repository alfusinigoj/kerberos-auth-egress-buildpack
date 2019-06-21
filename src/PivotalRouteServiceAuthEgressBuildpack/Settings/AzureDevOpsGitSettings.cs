using System;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Settings
{
    public class AzureDevOpsGitSettings : SettingsBase
    {
        const string AZURE_DEVOPS_COLL_URL_ENV_VAR_NM = "AZURE_DEVOPS_COLLECTION_URL";
        const string AZURE_DEVOPS_PROJECT_NM_ENV_VAR_NM = "AZURE_DEVOPS_PROJECT_NAME";
        const string AZURE_DEVOPS_REPO_NM_ENV_VAR_NM = "AZURE_DEVOPS_REPO_NAME";
        const string AZURE_DEVOPS_ACCESS_TOKEN_ENV_VAR_NM = "AZURE_DEVOPS_ACCESS_TOKEN";
        const string AZURE_DEVOPS_SRC_KEYTAB_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM = "AZURE_DEVOPS_SOURCE_KEYTAB_FILE_URL_RELATIVE_TO_THE_ROOT";
        const string AZURE_DEVOPS_SRC_KERBEROS_CONFIG_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM = "AZURE_DEVOPS_SOURCE_KERBEROS_CONFIG_FILE_URL_RELATIVE_TO_THE_ROOT";

        public static Uri CollectionUrl => new Uri(GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_DEVOPS_COLL_URL_ENV_VAR_NM)));
        public static string ProjectName => GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_DEVOPS_PROJECT_NM_ENV_VAR_NM));
        public static string RepositoryName => GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_DEVOPS_REPO_NM_ENV_VAR_NM));
        public static string KerberosKeytabPath => Environment.GetEnvironmentVariable(AZURE_DEVOPS_SRC_KEYTAB_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM);
        public static string KerberosConfigPath => Environment.GetEnvironmentVariable(AZURE_DEVOPS_SRC_KERBEROS_CONFIG_FILE_URL_RELATIVE_TO_THE_ROOT_ENV_VAR_NM);
        public static string AccessToken => GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_DEVOPS_ACCESS_TOKEN_ENV_VAR_NM));
    }
}
