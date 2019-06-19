namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public interface IGitFileDownloader
    {
        void Download(string targetFilePath);
    }
}
