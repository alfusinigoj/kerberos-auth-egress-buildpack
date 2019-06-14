using System;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public interface IConfigFileAppender : IDisposable
    {
        void Execute();
    }
}