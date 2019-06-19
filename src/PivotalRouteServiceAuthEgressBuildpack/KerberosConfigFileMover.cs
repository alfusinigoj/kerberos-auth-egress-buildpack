using System;
using System.IO;
using System.Xml;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class KerberosConfigFileMover : IKerberosConfigFileMover
    {
        private readonly string targetConfigFilePath;
        private readonly IGitFileDownloader downloader;
       

        public KerberosConfigFileMover(string targetConfigFilePath, IGitFileDownloader downloader)
        {
            this.targetConfigFilePath = targetConfigFilePath;
            this.downloader = downloader;
        }

        public void Move()
        {
            downloader.Download(targetConfigFilePath);
        }
    }

    public interface IKerberosConfigFileMover : IFileMover
    { }
}
