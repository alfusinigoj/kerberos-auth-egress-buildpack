using System;
using System.Xml;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class KeyTabFileMover : IKeyTabFileMover
    {
        private readonly string buildPath;
        private readonly IGitFileDownloader downloader;

        public KeyTabFileMover(string buildPath, IGitFileDownloader downloader)
        {
            this.buildPath = buildPath;
            this.downloader = downloader;
        }

        public void Move()
        {
            throw new NotImplementedException();
        }
    }

    public interface IKeyTabFileMover : IFileMover
    { }
}
