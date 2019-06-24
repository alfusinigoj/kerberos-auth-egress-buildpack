using Pivotal.RouteService.Auth.Egress.Buildpack.Downloaders;
using System;
using System.IO;
using Xunit;

namespace UnitTests
{
    public class AzureDevopsGitFileDownloaderTests
    {
        Uri collectionUrl = new Uri("https://foo.visualstudio.com");
        string projectName = "foo_bar";
        string repoName = "foo_bar_repo";
        string apiToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        string sourceFileUrlRelativeToTheRoot = "README.md";

        [Fact(Skip = "No test repository available")]
        public void Test_WithAPITokenProvided()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestAzureGitDownload", "AzureGit.txt");

            if (File.Exists(path))
                File.Delete(path);
                
            var downloader = new AzureDevopsGitFileDownloader(collectionUrl, projectName, repoName, apiToken, sourceFileUrlRelativeToTheRoot);
            downloader.Download(path);

            Assert.True(File.Exists(path));
            Assert.True(File.ReadAllText(path).Length > 0);
        }
    }
}
