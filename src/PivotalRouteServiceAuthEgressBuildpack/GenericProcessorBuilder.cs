using Pivotal.RouteService.Auth.Egress.Buildpack.Downloaders;
using Pivotal.RouteService.Auth.Egress.Buildpack.Settings;
using System;
using System.IO;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class GenericProcessorBuilder
    {
        private readonly string buildPath;

        public GenericProcessorBuilder(string buildPath)
        {
            this.buildPath = buildPath;
        }

        public IProcessor Build()
        {
            string kerberosConfigTargetFilePath = Path.Combine(buildPath, "bin", "krb5.ini");

            var kerberosConfigFileMover = GetKerberosConfigFileMover(kerberosConfigTargetFilePath);
            var kerberosConfigUpdater = new KerberosConfigUpdater(kerberosConfigTargetFilePath);
            var keyTabFileMover = GetKerberosKeytabFileMover();

            return new GenericProcessor(kerberosConfigFileMover, kerberosConfigUpdater, keyTabFileMover);
        }

        private IKeyTabFileMover GetKerberosKeytabFileMover()
        {
            IKeyTabFileMover keyTabFileMover;

            if (!string.IsNullOrWhiteSpace(GitHubSettings.KerberosKeytabRawUrlString))
            {
                keyTabFileMover = new KeyTabFileMover(buildPath, new GitHubFileDownloader(new Uri(GitHubSettings.KerberosKeytabRawUrlString), GitHubSettings.AccessToken));
            }
            else if (!string.IsNullOrWhiteSpace(AzureDevOpsGitSettings.KerberosKeytabPath))
            {
                var downloader = new AzureDevopsGitFileDownloader(AzureDevOpsGitSettings.CollectionUrl,
                                                                    AzureDevOpsGitSettings.ProjectName,
                                                                    AzureDevOpsGitSettings.RepositoryName,
                                                                    AzureDevOpsGitSettings.AccessToken,
                                                                    AzureDevOpsGitSettings.KerberosKeytabPath);

                keyTabFileMover = new KeyTabFileMover(buildPath, downloader);
            }
            else if (!string.IsNullOrWhiteSpace(AzureVaultSettings.SecretName))
            {
                var downloader = new AzureKeyVaultFileDownloader(AzureVaultSettings.VaultBaseUrl,
                                                                    AzureVaultSettings.ClientAppId,
                                                                    AzureVaultSettings.ClientAppSecret,
                                                                    AzureVaultSettings.SecretName,
                                                                    AzureVaultSettings.SecretVersion);

                keyTabFileMover = new KeyTabFileMover(buildPath, downloader);
            }
            else
            {
                throw new Exception("Kerberos keytab file information (GitHub/AzureDevOpsGit/AzureVault) not provided! Please refer to the readme for environment variable details to set.");
            }

            return keyTabFileMover;
        }

        private IKerberosConfigFileMover GetKerberosConfigFileMover(string targetPath)
        {
            IKerberosConfigFileMover kerberosConfigFileMover;

            if (!string.IsNullOrWhiteSpace(GitHubSettings.KerberosConfigRawUrlString))
            {
                kerberosConfigFileMover = new KerberosConfigFileMover(targetPath, new GitHubFileDownloader(new Uri(GitHubSettings.KerberosConfigRawUrlString), GitHubSettings.AccessToken));
            }
            else if (!string.IsNullOrWhiteSpace(AzureDevOpsGitSettings.KerberosConfigPath))
            {
                var downloader = new AzureDevopsGitFileDownloader(AzureDevOpsGitSettings.CollectionUrl,
                                                                    AzureDevOpsGitSettings.ProjectName,
                                                                    AzureDevOpsGitSettings.RepositoryName,
                                                                    AzureDevOpsGitSettings.AccessToken,
                                                                    AzureDevOpsGitSettings.KerberosConfigPath);

                kerberosConfigFileMover = new KerberosConfigFileMover(targetPath, downloader);
            }
            else
            {
                throw new Exception("Kerberos configuration information (GitHub/AzureDevOpsGit) not provided! Please refer to the readme for environment variable details to set.");
            }

            return kerberosConfigFileMover;
        }
    }
}
