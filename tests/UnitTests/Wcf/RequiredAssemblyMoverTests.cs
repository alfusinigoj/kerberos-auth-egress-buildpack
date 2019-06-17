using Pivotal.RouteService.Auth.Egress.Buildpack;
using Pivotal.RouteService.Auth.Egress.Buildpack.Wcf;
using System;
using System.IO;
using Xunit;

namespace UnitTests.Wcf
{
    public class RequiredAssemblyMoverTests
    {
        string testTargetPath = Path.Combine(Environment.CurrentDirectory, "targetBin");

        public RequiredAssemblyMoverTests()
        {
            Directory.CreateDirectory(testTargetPath);
        }

        [Fact]
        public void Test_IfAllRequiredAssembliesAreMovedToTheTargetBinFolder()
        {
            var requiredAssembliesDirectory = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\..\\..\\src\\requiredAssemblies");

            var mover = new RequiredAssemblyMover(requiredAssembliesDirectory, testTargetPath);
            mover.Move();

            var filesInTheSourceFolder = Directory.GetFiles(requiredAssembliesDirectory);

            foreach (var file in filesInTheSourceFolder)
            {
                Assert.True(File.Exists(Path.Combine(testTargetPath, Path.GetFileName(file))));
            }
        }
    }
}
