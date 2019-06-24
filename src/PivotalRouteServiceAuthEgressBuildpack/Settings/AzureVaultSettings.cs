using System;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Settings
{
    public class AzureVaultSettings : SettingsBase
    {
        const string AZURE_VAULT_BASE_URL_ENV_VAR_NM = "AZURE_VAULT_BASE_URL";
        const string AZURE_APP_ID_ENV_VAR_NM = "AZURE_APP_ID";
        const string AZURE_APP_SECRET_ENV_VAR_NM = "AZURE_APP_SECRET";
        const string AZURE_SECRET_NM_ENV_VAR_NM = "AZURE_SECRET_NM";
        const string AZURE_SECRET_VER_ENV_VAR_NM = "AZURE_SECRET_VER";

        public static Uri VaultBaseUrl => new Uri(GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_VAULT_BASE_URL_ENV_VAR_NM)));
        public static string ClientAppId => GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_APP_ID_ENV_VAR_NM));
        public static string ClientAppSecret => GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_APP_SECRET_ENV_VAR_NM));
        public static string SecretName => Environment.GetEnvironmentVariable(AZURE_SECRET_NM_ENV_VAR_NM);
        public static string SecretVersion => GetRequiredEnvironmentVariable(Environment.GetEnvironmentVariable(AZURE_SECRET_VER_ENV_VAR_NM));
    }
}
