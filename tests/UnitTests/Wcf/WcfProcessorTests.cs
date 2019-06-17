using Moq;
using Pivotal.RouteService.Auth.Egress.Buildpack;
using Pivotal.RouteService.Auth.Egress.Buildpack.Wcf;
using Xunit;

namespace UnitTests.Wcf
{
    public class WcfProcessorTests
    {
        Mock<IDetector> detector;
        Mock<IDependencyValidator> depValidator;
        Mock<IConfigFileAppender> configAppender;
        Mock<IAssemblyMover> assemMover;

        public WcfProcessorTests()
        {
            detector = new Mock<IDetector>();
            depValidator = new Mock<IDependencyValidator>();
            configAppender = new Mock<IConfigFileAppender>();
            assemMover = new Mock<IAssemblyMover>();
        }

        [Fact]
        public void Test_DoesNothingIfDetectorReturnsFalse()
        {
            detector.Setup(d => d.Find()).Returns(false);

            var processor = new WcfProcessor(detector.Object, depValidator.Object, configAppender.Object, assemMover.Object);

            processor.Execute();

            depValidator.Verify(dv => dv.Validate(), Times.Never);
            configAppender.Verify(c => c.Execute(), Times.Never);
            assemMover.Verify(a => a.Move(), Times.Never);
        }

        [Fact]
        public void Test_ExecutesEachExecutorsIfDetectorReturnsTrue()
        {
            detector.Setup(d => d.Find()).Returns(true);

            var processor = new WcfProcessor(detector.Object, depValidator.Object, configAppender.Object, assemMover.Object);

            processor.Execute();

            depValidator.Verify(dv => dv.Validate(), Times.Once);
            configAppender.Verify(c => c.Execute(), Times.Once);
            assemMover.Verify(a => a.Move(), Times.Once);
        }
    }
}
