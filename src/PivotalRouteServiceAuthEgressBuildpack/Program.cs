using System;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class Program
    {
        static int Main(string[] args)
        {
            return new EgressBuildpack().Run(args);
        }
    }
}