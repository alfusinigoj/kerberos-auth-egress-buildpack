using Pivotal.RouteService.Auth.Egress.Buildpack;
using System;
using System.IO;
using Xunit;

namespace UnitTests
{
    public class GitHubFileDownloaderTests
    {
        const string RAW_GIT_URL = "https://raw.githubusercontent.com/alfusinigoj/route-service-auth-egress-buildpack/master/README.md";
        const string NOT_RAW_GIT_URL = "https://github.com/alfusinigoj/route-service-auth-egress-buildpack/blob/master/README.md";

        [Fact]
        public void Test_WithAPITokenProvided()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestGitDownload", "From_Github");

            if (File.Exists(path))
                File.Delete(path);
                
            var downloader = new GitHubFileDownloader(RAW_GIT_URL, "50b82e65d6c79f77d1433fbdf9035b36ac698c50");
            downloader.Download(path);

            Assert.True(File.Exists(path));
            Assert.True(File.ReadAllText(path).Length > 0);

        }

        [Fact]
        public void Test_WithoutAPITokenProvided()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestGitDownload", "From_NoToken_Github");

            if (File.Exists(path))
                File.Delete(path);

            var downloader = new GitHubFileDownloader(RAW_GIT_URL);
            downloader.Download(path);


            Assert.True(File.Exists(path));
            Assert.True(File.ReadAllText(path).Length > 0);
        }

        [Fact]
        public void Test_ThrowsExceptionIfTheUrlProvidedIsNotRaw()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestGitDownload", "Foo");

            if (File.Exists(path))
                File.Delete(path);

            var downloader = new GitHubFileDownloader(NOT_RAW_GIT_URL);

            Assert.Throws<Exception>(()=> downloader.Download(path));
        }

        [Fact]
        public void Test_DeletesTheEmptyFileIfAnyExceptionIsThrown()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestGitDownload", "Bar");

            if (File.Exists(path))
                File.Delete(path);

            var downloader = new GitHubFileDownloader(NOT_RAW_GIT_URL);

            try
            {
                downloader.Download(path);
            }
            catch (Exception)
            {} 
            Assert.False(File.Exists(path));
        }
    }
}
