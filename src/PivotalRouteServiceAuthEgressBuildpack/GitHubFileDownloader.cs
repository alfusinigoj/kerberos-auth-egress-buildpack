using System;
using System.IO;
using System.Net;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class GitHubFileDownloader : IGitFileDownloader
    {
        const string AUTH_HEADER = "Authorization";
        const string ACCEPT_HEADER = "Accept";
        readonly Uri fileRawGitHubUrl;
        private readonly string apiToken;

        public GitHubFileDownloader(Uri fileRawGitHubUrl, string apiToken)
            : this(fileRawGitHubUrl)
        {
            this.apiToken = apiToken;
        }

        public GitHubFileDownloader(Uri fileRawGitHubUrl)
        {
            this.fileRawGitHubUrl = fileRawGitHubUrl;
        }

        public void Download(string targetFilePath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (!fileRawGitHubUrl.ToString().Contains("raw.githubusercontent.com"))
                throw new Exception($"{fileRawGitHubUrl} is not a valid github raw url!");

            Console.WriteLine($"-----> Downloading file {fileRawGitHubUrl} into {targetFilePath}...");

            using (var client = new WebClient())
            {
                if (apiToken != null)
                    client.Headers.Add(AUTH_HEADER, $"token {apiToken}");

                client.Headers.Add(ACCEPT_HEADER, $"application/vnd.github.v3.raw");

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                    File.WriteAllText(targetFilePath, string.Empty);

                    client.DownloadFile(fileRawGitHubUrl, targetFilePath);
                }
                catch (Exception ex)
                {
                    if (File.Exists(targetFilePath))
                        File.Delete(targetFilePath);

                    throw ex;
                }
                
            }
        }
    }
}
