using Pivotal.RouteService.Auth.Egress.Buildpack.Downloaders;
using System;
using System.IO;
using Xunit;

namespace UnitTests
{
    public class AzureKeyVaultFileDownloaderTests
    {
        private Uri vaultUrl= new Uri("https://dev.vault.azure.net");
        private string clientId = "yyyyyyyyyyyyyy";
        private string clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        private string secretName = "Test";
        private string secretVersion = "v1";

        //[Fact(Skip = "As random test only")]
        public void Test()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestAzureVaultDownload", "Secret.txt");

            if (File.Exists(path))
                File.Delete(path);
                
            var downloader = new AzureKeyVaultFileDownloader(vaultUrl, clientId, clientSecret, secretName, secretVersion);
            downloader.Download(path);

            Assert.True(File.Exists(path));
            Assert.True(File.ReadAllText(path).Length > 0);
        }
    }
}
