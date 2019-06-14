using System;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public abstract class FinalBuildpack : BuildpackBase
    {
        /// <summary>
        /// Determines the startup command for the app
        /// </summary>
        /// <param name="buildPath">Directory path to the application</param>
        /// <returns>Startup command executed by Cloud Foundry to launch the application</returns>
        public abstract string GetStartupCommand(string buildPath);
        protected override int DoRun(string[] args)
        {
            var command = args[0];
            switch (command)
            {
                case "finalize":
                    Apply(args[1], args[2], args[3], int.Parse(args[4]));
                    break;
                case "release":
                    Console.WriteLine("default_process_types:");
                    Console.WriteLine($"  web: {GetStartupCommand(args[1])}");
                    break;
                default:
                    return base.DoRun(args);
            }

            return 0;
        }
    }
}