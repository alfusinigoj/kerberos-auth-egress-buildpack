using Pivotal.RouteService.Auth.Egress.Buildpack.Wcf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class EgressBuildpack : SupplyBuildpack
    {

        protected override bool Detect(string buildPath)
        {
            return false;
        }

        protected override void Apply(string buildPath, string cachePath, string depsPath, int index)
        {
            try
            {
                Console.WriteLine("=======================================================================================");
                Console.WriteLine("============= Route Service Iwa Auth Egress Buildpack execution started ===============");
                Console.WriteLine("=======================================================================================");

                var webConfigPath = Path.Combine(buildPath, "web.config");

                if (!File.Exists(webConfigPath))
                {
                    Console.WriteLine("-----> Web.config file not found, so skipping ececution...");
                    return;
                }

                foreach (var processor in GetProcessors(webConfigPath, buildPath))
                {
                    processor.Execute();
                }


                Console.WriteLine("=======================================================================================");
                Console.WriteLine("============= Route Service Iwa Auth Egress Buildpack execution completed =============");
                Console.WriteLine("=======================================================================================");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"-----> **ERROR** Buildpack execution failed with exception, {exception}");
                Environment.Exit(-1);
            }
        }

        private List<IProcessor> GetProcessors(string webConfigPath, string buildPath)
        {
            var processors = new List<IProcessor>()
            {
                new WcfProcessor(new ClientDetector(webConfigPath),
                                                new DependencyValidator(buildPath),
                                                new WebConfigFileAppender(webConfigPath),
                                                new RequiredAssemblyMover(Path.Combine(Path.GetDirectoryName(typeof(EgressBuildpack).Assembly.Location), "requiredAssemblies"), Path.Combine(buildPath, "bin")))
            };

            return processors;
        }
    }
}
