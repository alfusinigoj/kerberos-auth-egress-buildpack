using Pivotal.RouteService.Auth.Egress.Buildpack.Wcf;
using System;
using System.IO;
using Xunit;

namespace UnitTests.Wcf
{
    public class ClientDetectorTests
    {
        string testFilePath = Path.Combine(Environment.CurrentDirectory, "config", "test_web.config");

        string clientExistsConfigText = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration>  <system.serviceModel><client><endpoint address=\"http://foo1/service.svc\" name=\"foo\" ></endpoint></client></system.serviceModel></configuration>";
        string clientNotExistsConfigText = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration>  <system.serviceModel></system.serviceModel></configuration>";

        public ClientDetectorTests()
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "config"));
        }

        [Fact]
        public void Test_IfDetectorReturnsTrueIfWcfClientExists()
        {
            File.WriteAllText(testFilePath, clientExistsConfigText);
            var detector = new ClientDetector(testFilePath);
            Assert.True(detector.Find());
        }

        [Fact]
        public void Test_IfDetectorReturnsFalseIfWcfClientSectionDoesNotExists()
        {
            File.WriteAllText(testFilePath, clientNotExistsConfigText);
            var detector = new ClientDetector(testFilePath);
            Assert.False(detector.Find());
        }
    }
}
