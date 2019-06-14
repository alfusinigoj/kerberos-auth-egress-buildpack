using System;
using System.Xml;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Wcf
{
    public class ClientDetector : IDetector
    {
        XmlDocument doc = new XmlDocument();

        public ClientDetector(string webConfigPath)
        {
            doc.Load(webConfigPath);
        }

        public bool Find()
        {
            Console.WriteLine("-----> Checking for WCF Client sections in web.config...");

            var client = doc.SelectSingleNode("configuration/system.serviceModel/client");

            if (client == null)
            {
                Console.WriteLine($"-----> **INFO** WCF Client not found, skipping configuring egress modules for WCF Clients");
                return false;
            }

            Console.WriteLine("-----> Detected WCF Client sections in this application's config");
            return true;
        }
    }
}
