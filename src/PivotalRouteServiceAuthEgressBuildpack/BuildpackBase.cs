using System;
using System.Collections;
using System.Linq;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public abstract class BuildpackBase
    {
        /// <summary>
        /// Determines if the buildpack is compatible and should be applied to the application being staged 
        /// </summary>
        /// <param name="buildPath">Directory path to the application</param>
        /// <returns>True if buildpack should be applied, otherwise false</returns>
        protected abstract bool Detect(string buildPath);

        /// <summary>
        /// Logic to apply when buildpack is ran.
        /// Note that for <see cref="SupplyBuildpack"/> this will correspond to "bin/supply" lifecycle event, while for <see cref="FinalBuildpack"/> it will be invoked on "bin/finalize"
        /// </summary>
        /// <param name="buildPath">Directory path to the application</param>
        /// <param name="cachePath">Location the buildpack can use to store assets during the build process</param>
        /// <param name="depsPath">Directory where dependencies provided by all buildpacks are installed. New dependencies introduced by current buildpack should be stored inside subfolder named with index argument ({depsPath}/{index})</param>
        /// <param name="index">Number that represents the ordinal position of the buildpack</param>
        protected abstract void Apply(string buildPath, string cachePath, string depsPath, int index);

        /// <summary>
        /// Entry point into the buildpack. Should be called from Main method with args
        /// </summary>
        /// <param name="args">Args array passed into Main method</param>
        /// <returns>Status return code</returns>
        public int Run(string[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            foreach (var e in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
            {
                Console.WriteLine($"{e.Key}: {e.Value}");
            }
            return DoRun(args);
        }

        protected virtual int DoRun(string[] args)
        {
            var command = args[0];
            switch (command)
            {
                case "detect":
                    return Detect(args[1]) ? 2 : 1;
            }

            return 0;
        }

    }
}