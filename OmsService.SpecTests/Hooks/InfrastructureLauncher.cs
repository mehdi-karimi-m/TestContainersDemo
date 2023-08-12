using BoDi;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using OmsService.SpecTests.Support;

namespace OmsService.SpecTests.Hooks
{
    [Binding]
    internal class InfrastructureLauncher
    {
        private readonly IObjectContainer _objectContainer;
        private INetwork _network;
        private MsSqlService _msSqlService;
        private TestContainerWebApiService _testContainerWebApiService;
        public InfrastructureLauncher(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public async Task Launch()
        {
            _network = new NetworkBuilder()
                .WithName(Guid.NewGuid().ToString("D"))
                .Build();
            await _network.CreateAsync().ConfigureAwait(false);

            _msSqlService = await MsSqlService.Create(_network);

            var omsServiceWebApiEnvironments = new Dictionary<string, string>
            {
                { "ConnectionStrings__OmsDbConnection", _msSqlService.InsideOfTestContainersConnectionString }
            };
            _testContainerWebApiService = await OmsServiceWebApiFactory.Create("Dockerfile", _network, omsServiceWebApiEnvironments);
            _objectContainer.RegisterInstanceAs(_network);
            _objectContainer.RegisterInstanceAs(_msSqlService);
            _objectContainer.RegisterInstanceAs(_testContainerWebApiService);
        }

        [AfterTestRun]
        public async Task CleanUp()
        {
            _testContainerWebApiService?.Dispose();
            _msSqlService?.Dispose();
            if (_network != null) await _network.DisposeAsync().ConfigureAwait(false);
        }
    }
}
