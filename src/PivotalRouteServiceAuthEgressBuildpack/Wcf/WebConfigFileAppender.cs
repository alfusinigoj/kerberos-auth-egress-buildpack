using Pivotal.RouteServiceIwaWcfInterceptor;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Pivotal.RouteService.Auth.Egress.Buildpack.Wcf
{
    public class WebConfigFileAppender : IConfigFileAppender
    {
        private bool disposedValue = false;
        private readonly string webConfigPath;
        private readonly string buildPath;
        List<string> svcEndpointLevelBehaviours = new List<string>();
        List<string> clientEndpointLevelBehaviours = new List<string>();
        XmlDocument doc = new XmlDocument();

        public WebConfigFileAppender(string webConfigPath, string buildPath)
        {
            this.webConfigPath = webConfigPath;
            this.buildPath = buildPath;
            doc.Load(webConfigPath);
        }

        public void Execute()
        {
            Console.WriteLine("-----> Applying configuration changes to add RouteServiceIwaWcfInterceptor, from nuget package PivotalServices.WcfClient.Kerberos.Interceptor into the egress pipeline...");

            AddAllExistingSvcEndpointBehaviours(doc, svcEndpointLevelBehaviours);

            var client = doc.SelectSingleNode("configuration/system.serviceModel/client");

            var endpoints = client.SelectNodes("endpoint");

            AddAllExistingClientEndpointBehaviours(clientEndpointLevelBehaviours, endpoints);

            var serviceModel = doc.SelectSingleNode("configuration/system.serviceModel");

            var behaviours = GetOrCreateBehavioursSection(doc, serviceModel);

            var endpointBehaviours = GetOrCreateEndPointBehaviours(doc, behaviours);

            var individualBehaviours = endpointBehaviours.SelectNodes("behavior");

            var pivotalWcfClientIwaInterceptorBehaviourExists = PivotalWcfClientIwaInterceptorBehaviourExistsAlready(individualBehaviours);

            if (!pivotalWcfClientIwaInterceptorBehaviourExists)
                CreatePivotalWcfClientIwaInterceptorBehaviour(doc, endpointBehaviours);

            SetPivotalWcfClientIwaInterceptorBehaviourToAllEndpoints(doc, client);

            AddPivotalWcfClientIwaInterceptorExtensionsToPreExistingClientEndpointBehaviours(doc, svcEndpointLevelBehaviours, clientEndpointLevelBehaviours, individualBehaviours);

            ApplyBehaviourConfigurationToEndpointsNotHavingBahaviourConfiguredAlready(doc, endpoints);

            var extensions = GetOrCreateExtensionsNode(doc, serviceModel);

            var behaviorExtensions = GetOrCreateBehaviourExtensionsNode(doc, extensions);

            var pivotalIwaInterceptorExtensions = behaviorExtensions.SelectNodes("add");

            var isExtensionExist = PivotalWcfClientIwaInterceptorExtensionsExists(pivotalIwaInterceptorExtensions);

            if (!isExtensionExist)
                CreatePivotalWcfClientIwaInterceptorExtensions(doc, behaviorExtensions);

            ValidateIfAllEndPointsAreSetWithBehaviourConfiguration(client);
        }

        private XmlNode AddInterceptorExtension(XmlDocument xmlDoc)
        {
            var extension = xmlDoc.CreateElement("pivotalWcfClientIwaInterceptorExtensions");
            return extension;
        }

        private static void ValidateIfAllEndPointsAreSetWithBehaviourConfiguration(XmlNode client)
        {
            var endPoints = client.SelectNodes("endpoint");

            bool behaviourExist = true;

            for (int i = 0; i < endPoints.Count; i++)
            {
                if (endPoints.Item(i).Attributes["behaviorConfiguration"] == null)
                    behaviourExist = false;
            }

            if (!behaviourExist)
                Console.Error.WriteLine(@"-----> **WARNING** One or more of the client\endpoint does not have a behaviorConfiguration set!");
        }


        private static XmlElement GetOrCreateBehavioursSection(XmlDocument doc, XmlNode serviceModel)
        {
            var behaviours = (XmlElement)serviceModel.SelectSingleNode("behaviors");

            if (behaviours == null)
            {
                behaviours = doc.CreateElement("behaviors");
                serviceModel.AppendChild(behaviours);
            }

            return behaviours;
        }

        private static void CreatePivotalWcfClientIwaInterceptorExtensions(XmlDocument doc, XmlElement behaviorExtensions)
        {
            var interceptorExtensionNode = doc.CreateElement("add");
            interceptorExtensionNode.SetAttribute("name", "pivotalWcfClientIwaInterceptorExtensions");
            interceptorExtensionNode.SetAttribute("type", typeof(IwaInterceptorBehaviourExtensionElement).AssemblyQualifiedName);
            behaviorExtensions.AppendChild(interceptorExtensionNode);
        }

        private static bool PivotalWcfClientIwaInterceptorExtensionsExists(XmlNodeList pivotalIwaInterceptorExtensions)
        {
            bool isExtensionExist = false;

            for (int i = 0; i < pivotalIwaInterceptorExtensions.Count; i++)
            {
                if(pivotalIwaInterceptorExtensions.Item(i).Attributes["name"] == null)
                    throw new Exception(@"-----> **ERROR** One or more of endpoint behaviours are without the attribute 'name'!");

                if (pivotalIwaInterceptorExtensions.Item(i).Attributes["name"].Value == "pivotalWcfClientIwaInterceptorExtensions")
                    isExtensionExist = true;
            }

            return isExtensionExist;
        }

        private static XmlElement GetOrCreateBehaviourExtensionsNode(XmlDocument doc, XmlElement extensions)
        {
            var behaviorExtensions = (XmlElement)extensions.SelectSingleNode("behaviorExtensions");

            if (behaviorExtensions == null)
            {
                behaviorExtensions = doc.CreateElement("behaviorExtensions");
                extensions.AppendChild(behaviorExtensions);
            }

            return behaviorExtensions;
        }

        private static XmlElement GetOrCreateExtensionsNode(XmlDocument doc, XmlNode serviceModel)
        {
            var extensions = (XmlElement)serviceModel.SelectSingleNode("extensions");

            if (extensions == null)
            {
                extensions = doc.CreateElement("extensions");
                serviceModel.AppendChild(extensions);
            }

            return extensions;
        }

        private static void ApplyBehaviourConfigurationToEndpointsNotHavingBahaviourConfiguredAlready(XmlDocument doc, XmlNodeList endpoints)
        {
            for (int i = 0; i < endpoints.Count; i++)
            {
                var elbc = endpoints.Item(0).Attributes["behaviorConfiguration"]?.Value;

                if (string.IsNullOrWhiteSpace(elbc))
                {
                    var behaviourConfigurationAttribute = doc.CreateAttribute("behaviorConfiguration");
                    behaviourConfigurationAttribute.Value = "pivotalWcfClientIwaInterceptorBehaviour";
                    endpoints.Item(i).Attributes.Append(behaviourConfigurationAttribute);
                }
            }
        }

        private void AddPivotalWcfClientIwaInterceptorExtensionsToPreExistingClientEndpointBehaviours(XmlDocument doc, List<string> svcEndpointLevelBehaviours, List<string> clientEndpointLevelBehaviours, XmlNodeList individualBehaviours)
        {
            for (int i = 0; i < individualBehaviours.Count; i++)
            {
                if (individualBehaviours.Item(i).Attributes["name"] == null)
                    throw new Exception(@"-----> **ERROR** One or more of endpoint behaviours are without the attribute 'name'!");

                var behaviourName = individualBehaviours.Item(i).Attributes["name"].Value;

                if (svcEndpointLevelBehaviours.Contains(behaviourName) && clientEndpointLevelBehaviours.Contains(behaviourName))
                {
                    Console.Error.WriteLine($"EndPointBehaviour '{behaviourName}' is shared by client and service. Please split them and continue!");
                    Environment.Exit(-1);
                }
                else if (!svcEndpointLevelBehaviours.Contains(behaviourName))
                {
                    if (individualBehaviours.Item(i).SelectSingleNode("pivotalWcfClientIwaInterceptorExtensions") == null)
                    {
                        individualBehaviours.Item(i).AppendChild(AddInterceptorExtension(doc));
                    }
                }
            }
        }

        private static void SetPivotalWcfClientIwaInterceptorBehaviourToAllEndpoints(XmlDocument doc, XmlNode client)
        {
            var endPoints = client.SelectNodes("endpoint");

            for (int i = 0; i < endPoints.Count; i++)
            {
                if (endPoints.Item(i).Attributes["behaviorConfiguration"] == null)
                {
                    var behaviourConfigurationAttribute = doc.CreateAttribute("behaviorConfiguration");
                    behaviourConfigurationAttribute.Value = "pivotalWcfClientIwaInterceptorBehaviour";
                    endPoints.Item(i).Attributes.Append(behaviourConfigurationAttribute);
                }
            }
        }

        private void CreatePivotalWcfClientIwaInterceptorBehaviour(XmlDocument doc, XmlElement endpointBehaviours)
        {
            var pivotalWcfClientIwaInterceptorBehaviour = doc.CreateElement("behavior");
            pivotalWcfClientIwaInterceptorBehaviour.SetAttribute("name", "pivotalWcfClientIwaInterceptorBehaviour");
            pivotalWcfClientIwaInterceptorBehaviour.AppendChild(AddInterceptorExtension(doc));
            endpointBehaviours.AppendChild(pivotalWcfClientIwaInterceptorBehaviour);
        }

        private static bool PivotalWcfClientIwaInterceptorBehaviourExistsAlready(XmlNodeList individualBehaviours)
        {
            bool pivotalWcfClientIwaInterceptorBehaviourExists = false;

            for (int i = 0; i < individualBehaviours.Count; i++)
            {
                if (individualBehaviours.Item(i).Attributes["name"] == null)
                    throw new Exception(@"-----> **ERROR** One or more of endpoint behaviours are without the attribute 'name'!");

                if (individualBehaviours.Item(i).Attributes["name"].Value == "pivotalWcfClientIwaInterceptorBehaviour")
                    pivotalWcfClientIwaInterceptorBehaviourExists = true;
            }

            return pivotalWcfClientIwaInterceptorBehaviourExists;
        }

        private static XmlElement GetOrCreateEndPointBehaviours(XmlDocument doc, XmlElement behaviours)
        {
            var endpointBehaviours = (XmlElement)behaviours.SelectSingleNode("endpointBehaviors");

            if (endpointBehaviours == null)
            {
                endpointBehaviours = doc.CreateElement("endpointBehaviors");
                behaviours.AppendChild(endpointBehaviours);
            }

            return endpointBehaviours;
        }

        private static void AddAllExistingClientEndpointBehaviours(List<string> clientEndpointLevelBehaviours, XmlNodeList endpoints)
        {
            for (int j = 0; j < endpoints.Count; j++)
            {
                var elbc = endpoints.Item(0).Attributes["behaviorConfiguration"]?.Value;
                if (!string.IsNullOrWhiteSpace(elbc))
                    clientEndpointLevelBehaviours.Add(elbc);
            }
        }

        private static void AddAllExistingSvcEndpointBehaviours(XmlDocument doc, List<string> svcEndpointLevelBehaviours)
        {
            var individualServices = doc.SelectNodes("configuration/system.serviceModel/services/service");
            for (int i = 0; i < individualServices.Count; i++)
            {
                var svcendpoints = individualServices.Item(i).SelectNodes("endpoint");

                for (int j = 0; j < svcendpoints.Count; j++)
                {
                    var elbc = svcendpoints.Item(0).Attributes["behaviorConfiguration"]?.Value;
                    if (!string.IsNullOrWhiteSpace(elbc))
                        svcEndpointLevelBehaviours.Add(elbc);
                }
            }
        }


        private void SaveChanges()
        {
            doc.Save(webConfigPath);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SaveChanges();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
