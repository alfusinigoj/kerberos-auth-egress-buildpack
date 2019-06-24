using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Downloaders
{
    public class AzureKeyVaultFileDownloader : IGitFileDownloader
    {
        private readonly Uri vaultUrl;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string secretName;
        private readonly string secretVersion;

        public AzureKeyVaultFileDownloader(Uri vaultUrl, string clientId, string clientSecret, string secretName, string secretVersion)
        {
            this.vaultUrl = vaultUrl;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.secretName = secretName;
            this.secretVersion = secretVersion;
        }

        public void Download(string targetFilePath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //Console.WriteLine($"-----> Downloading file {sourceFileUrlRelativeToTheRoot} into {targetFilePath} from azure devops collection: {collectionUrl}, project: {projectName}, repo: {repoName}");

            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));

            var secret = Task.Run(() => client.GetSecretAsync(vaultUrl.AbsoluteUri, secretName, secretVersion)).ConfigureAwait(false).GetAwaiter().GetResult();

            File.WriteAllBytes(targetFilePath, Convert.FromBase64String(secret.Value));
        }

        public async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException($"Failed to obtain Access token for the client {clientId}");

            return result.AccessToken;
        }
    }
}
