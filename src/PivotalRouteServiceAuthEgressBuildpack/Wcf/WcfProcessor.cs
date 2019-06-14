namespace Pivotal.RouteService.Auth.Egress.Buildpack.Wcf
{
    public class WcfProcessor : IProcessor
    {
        private readonly IDetector detector;
        private readonly IDependencyValidator dependencyValidator;
        private readonly IConfigFileAppender fileAppender;
        private readonly IAssemblyMover assemblyMover;

        public WcfProcessor(IDetector detector, IDependencyValidator dependencyValidator, IConfigFileAppender fileAppender, IAssemblyMover assemblyMover)
        {
            this.detector = detector;
            this.dependencyValidator = dependencyValidator;
            this.fileAppender = fileAppender;
            this.assemblyMover = assemblyMover;
        }

        public void Execute()
        {
            if(detector.Find())
            {
                dependencyValidator.Validate();

                using (fileAppender)
                    fileAppender.Execute();

                assemblyMover.Move();
            }
        }
    }
}
