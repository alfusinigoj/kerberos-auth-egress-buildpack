using System;
using System.IO;
using System.Xml;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class KeyTabFileMover : IKeyTabFileMover
    {
        private readonly string buildPath;
        private readonly IGitFileDownloader downloader;
        const string KEYTAB_FILE_NM = "creds.ccreds";

        public KeyTabFileMover(string buildPath, IGitFileDownloader downloader)
        {
            this.buildPath = buildPath;
            this.downloader = downloader;
        }

        public void Move()
        {
            var targetFileName = Path.Combine(buildPath, KEYTAB_FILE_NM);

            downloader.Download(targetFileName);
        }
    }

    public interface IKeyTabFileMover : IFileMover
    { }
}
