using DotNet.Testcontainers.Networks;
using System.Text.RegularExpressions;
using Testcontainers.MsSql;

namespace OmsService.SpecTests.Support
{
    internal class MsSqlService : IDisposable
    {
        public string OutsideOfTestContainersConnectionString { get; private set; }
        public string InsideOfTestContainersConnectionString { get; private set; }
        public string NetworkHostName { get; private set; }
        public INetwork Network { get; private set; }

        private static uint _instanceNumber = 0;
        private MsSqlContainer _container;

        private MsSqlService(INetwork network, MsSqlContainer msSqlContainer)
        {
            Network = network;
            _container = msSqlContainer;
            NetworkHostName = $"SqlServerHost{_instanceNumber}";
            OutsideOfTestContainersConnectionString = msSqlContainer.GetConnectionString();
            InsideOfTestContainersConnectionString = Regex.Replace(OutsideOfTestContainersConnectionString, "(Server=\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3},\\d+;)", $"Server={NetworkHostName};");
        }

        public static async Task<MsSqlService> Create(INetwork network)
        {
            _instanceNumber++;
            var networkHostName = $"SqlServerHost{_instanceNumber}";

            var container = new MsSqlBuilder()
                                      .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
                                      .WithNetwork(network)
                                      .WithNetworkAliases(networkHostName)
                                      .Build();
            await container.StartAsync();

            return new MsSqlService(network, container);
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }
        private async Task DisposeAsync()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}
