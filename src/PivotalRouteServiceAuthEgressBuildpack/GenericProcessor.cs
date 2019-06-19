namespace Pivotal.RouteService.Auth.Egress.Buildpack
{
    public class GenericProcessor : IProcessor
    {
        private readonly IKerberosConfigFileMover configFileMover;
        private readonly IKerberosConfigUpdater configUpdater;
        private readonly IKeyTabFileMover ketabMover;

        public GenericProcessor(IKerberosConfigFileMover configFileMover, IKerberosConfigUpdater configUpdater, IKeyTabFileMover ketabMover)
        {
            this.configFileMover = configFileMover;
            this.configUpdater = configUpdater;
            this.ketabMover = ketabMover;
        }

        public void Execute()
        {
            configFileMover.Move();
            configUpdater.Update();
            ketabMover.Move();
        }
    }
}
