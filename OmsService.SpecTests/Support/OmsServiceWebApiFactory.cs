using DotNet.Testcontainers.Builders;
using FizzWare.NBuilder;
using DotNet.Testcontainers.Networks;

namespace OmsService.SpecTests.Support
{
    internal static class OmsServiceWebApiFactory
    {
        public static async Task<TestContainerWebApiService> Create(string dockerFileName, INetwork network, IReadOnlyDictionary<string, string> environments)
        {
            var futureImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                .WithDockerfile(dockerFileName)
                .WithCleanUp(true)
                .Build();

            await futureImage.CreateAsync()
            .ConfigureAwait(false);

            var randomGenerator = new RandomGenerator(DateTime.Now.Millisecond);
            var httpPort = randomGenerator.Next(1000, 6000);

            var containerBuilder = new ContainerBuilder()
              .WithName(Guid.NewGuid().ToString("D"))
              .WithImage(futureImage.FullName).WithPortBinding(httpPort, true)
              .WithNetwork(network);
            containerBuilder.WithEnvironment(environments);
            containerBuilder.WithEnvironment("Kestrel__EndPoints__Http__Url", $"http://+:{httpPort}");

            var myServiceContainer = containerBuilder.Build();

            await myServiceContainer.StartAsync()
              .ConfigureAwait(false);

            return new TestContainerWebApiService(myServiceContainer, httpPort);
        }
    }
}
