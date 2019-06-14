using System;
using System.IO;
using System.Linq;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Wcf
{
    public class DependencyValidator : IDependencyValidator
    {
        private readonly string buildPath;

        public DependencyValidator(string buildPath)
        {
            this.buildPath = buildPath;
        }

        public void Validate()
        {
            var dir = new DirectoryInfo(buildPath);
            if (dir.EnumerateFiles("RouteServiceIwaWcfInterceptor.dll", SearchOption.AllDirectories).ToList().Count == 0
                || dir.EnumerateFiles("Microsoft.AspNetCore.Authentication.GssKerberos.dll", SearchOption.AllDirectories).ToList().Count == 0)
            {
                throw new Exception("-----> **ERROR** Could not find assembly 'RouteServiceIwaWcfInterceptor' or one/more of its dependencies, make sure to install the latest package 'PivotalServices.WcfClient.Kerberos.Interceptor' from https://www.nuget.org or https://www.myget.org/F/ajaganathan/api/v3/index.json");
            }
        }
    }
}
