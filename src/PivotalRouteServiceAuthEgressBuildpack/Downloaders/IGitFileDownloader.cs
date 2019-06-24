namespace Pivotal.RouteService.Auth.Egress.Buildpack.Downloaders
{
    public interface IGitFileDownloader
    {
        void Download(string targetFilePath);
    }
}
