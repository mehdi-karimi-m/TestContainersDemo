using DotNet.Testcontainers.Containers;

namespace OmsService.SpecTests.Support
{
    internal class TestContainerWebApiService : IDisposable
    {
        public Uri BaseAddress { get; private set; }
        private readonly IContainer _container;

        public TestContainerWebApiService(IContainer container, int serviceHttpPort)
        {
            _container = container;
            var uriBuilder = new UriBuilder("http", container.Hostname, container.GetMappedPublicPort(serviceHttpPort));
            BaseAddress = uriBuilder.Uri;
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }
        private async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}
