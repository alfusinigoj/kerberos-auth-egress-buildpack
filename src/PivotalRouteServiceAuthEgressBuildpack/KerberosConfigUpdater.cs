using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class KerberosConfigUpdater : IKerberosConfigUpdater
    {
        private readonly string kerberosConfigFilePath;

        public KerberosConfigUpdater(string kerberosConfigFilePath)
        {
            this.kerberosConfigFilePath = kerberosConfigFilePath;
        }

        public void Update()
        {
            var krbRawData = File.ReadAllText(kerberosConfigFilePath);
            var krbNewData = replaceKeyValue(krbRawData, "default_client_keytab_name", @"C:\Users\vcap\app\creds.ccreds");
            krbNewData = replaceKeyValue(krbNewData, "default_keytab_name", @"C:\Users\vcap\app\creds.ccreds");
            krbNewData = replaceKeyValue(krbNewData, "default_ccache_name", @"C:\Users\vcap\app\creds.ccache");

            File.WriteAllText(kerberosConfigFilePath, krbNewData);
        }

        string replaceKeyValue(string krbData, string keyName, string value)
        {
            var keyPattern = @"({0}\s?=\s?)([a-zA-Z0-9\\:.]*)";
            var matchKeyTabName = Regex.Match(krbData, string.Format(keyPattern, keyName));
            if (!matchKeyTabName.Success)
            {
                throw new ApplicationException($"No {keyName} found. Please refer to the MIT kerberos config template file.");
            }

            return Regex.Replace(krbData, string.Format(keyPattern, keyName), $"$1{value}");
        }
    }
}
