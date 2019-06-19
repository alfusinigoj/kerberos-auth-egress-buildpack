using System;
using System.IO;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class RequiredAssemblyMover : IFileMover
    {
        private readonly string sourcePath;
        private readonly string appBinPath;

        public RequiredAssemblyMover(string sourcePath, string appBinPath)
        {
            this.sourcePath = sourcePath;
            this.appBinPath = appBinPath;
        }

        public void Move()
        {
            string[] files = Directory.GetFiles(sourcePath);

            Console.WriteLine("-----> Injecting MIT Kerberos assembllies and c++ redistributables into the application target directory...");

            foreach (string file in files)
                File.Copy(file, Path.Combine(appBinPath, Path.GetFileName(file)), true);
        }
    }
}
