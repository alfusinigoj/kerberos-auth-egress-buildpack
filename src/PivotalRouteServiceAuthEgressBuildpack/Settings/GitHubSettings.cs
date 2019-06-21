using System;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Settings
{
    public class GitHubSettings : SettingsBase
    {
        const string GITHUB_KEYTAB_FILE_RAW_URL_ENV_VAR_NM = "GITHUB_KEYTAB_FILE_RAW_URL";
        const string GITHUB_KERBEROS_CONFIG_FILE_RAW_URL_ENV_VAR_NM = "GITHUB_KERBEROS_CONFIG_FILE_RAW_URL";
        const string GITHUB_ACCESS_TOKEN_ENV_VAR_NM = "GITHUB_ACCESS_TOKEN";

        public static string KerberosKeytabRawUrlString => Environment.GetEnvironmentVariable(GITHUB_KEYTAB_FILE_RAW_URL_ENV_VAR_NM);
        public static string KerberosConfigRawUrlString => Environment.GetEnvironmentVariable(GITHUB_KERBEROS_CONFIG_FILE_RAW_URL_ENV_VAR_NM);
        public static string AccessToken => Environment.GetEnvironmentVariable(GITHUB_ACCESS_TOKEN_ENV_VAR_NM);
    }
}
