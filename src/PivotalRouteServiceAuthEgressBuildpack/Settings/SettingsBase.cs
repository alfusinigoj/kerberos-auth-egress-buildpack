using System;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Settings
{
    public class SettingsBase
    {
        protected static string GetRequiredEnvironmentVariable(string variableName)
        {
            var value = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{variableName} environemt variable is not set!");

            return value;

        }
    }
}