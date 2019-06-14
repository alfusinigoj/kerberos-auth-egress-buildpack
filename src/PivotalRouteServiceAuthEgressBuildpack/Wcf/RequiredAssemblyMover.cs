using System;
using System.IO;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Wcf
{
    public class RequiredAssemblyMover : IAssemblyMover
    {
        private readonly string buildPath;

        public RequiredAssemblyMover(string buildPath)
        {
            this.buildPath = buildPath;
        }

        public void Move()
        {
            string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(typeof(EgressBuildpack).Assembly.Location), "requiredAssemblies"));

            Console.WriteLine("-----> Injecting MIT Kerberos assembllies and c++ redistributables into the application target directory...");

            foreach (string file in files)
                File.Copy(file, Path.Combine(buildPath, "bin", Path.GetFileName(file)), true);
        }
    }
}
