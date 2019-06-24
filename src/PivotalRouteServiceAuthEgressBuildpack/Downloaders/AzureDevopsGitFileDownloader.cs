using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Net;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Downloaders
{
    public class AzureDevopsGitFileDownloader : IGitFileDownloader
    {
        private readonly Uri collectionUrl;
        private readonly string projectName;
        private readonly string repoName;
        private readonly string apiToken;
        private readonly string sourceFileUrlRelativeToTheRoot;

        public AzureDevopsGitFileDownloader(Uri collectionUrl, string projectName, string repoName, string apiToken, string sourceFileUrlRelativeToTheRoot)
        {
            this.collectionUrl = collectionUrl;
            this.projectName = projectName;
            this.repoName = repoName;
            this.apiToken = apiToken;
            this.sourceFileUrlRelativeToTheRoot = sourceFileUrlRelativeToTheRoot;
        }

        public void Download(string targetFilePath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Console.WriteLine($"-----> Downloading file {sourceFileUrlRelativeToTheRoot} into {targetFilePath} from azure devops collection: {collectionUrl}, project: {projectName}, repo: {repoName}");

            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

            var credentials = new VssCredentials(new VssBasicCredential("", apiToken));

            var connection = new VssConnection(collectionUrl, credentials);

            var client = connection.GetClient<GitHttpClient>();

            var repository = client.GetRepositoryAsync(projectName, repoName, string.Empty, default).Result;

            var descriptor = new GitVersionDescriptor() { Version = "master" };

            using (var file = client.GetItemContentAsync(repository.Id, sourceFileUrlRelativeToTheRoot, versionDescriptor: descriptor).Result)
            {
                using (var buffer = new MemoryStream())
                {
                    file.CopyTo(buffer);
                    File.WriteAllBytes(targetFilePath, buffer.ToArray());
                }
            }
        }
    }
}
