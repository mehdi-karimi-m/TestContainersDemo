using DotNet.Testcontainers.Builders;
using FizzWare.NBuilder;
using FluentAssertions;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Testcontainers.MsSql;

namespace SimpleMsSqlExample
{
    public class RunAnWebApiTests
    {
        [Fact]
        public async Task Run_an_web_api_from_docker_file()
        {
            MsSqlContainer? msSqlContainer = null;
            try
            {
                const string sqlServerHost = "SqlServerHost";
                var network = new NetworkBuilder()
                                  .WithName(Guid.NewGuid().ToString("D"))
                                  .Build();

                await network.CreateAsync().ConfigureAwait(false);

                msSqlContainer = new MsSqlBuilder()
                                      .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
                                      .WithNetwork(network)
                                      .WithNetworkAliases(sqlServerHost)
                                      .Build();


                await msSqlContainer.StartAsync();

                var connectionString = msSqlContainer.GetConnectionString();

                var d = CommonDirectoryPath.GetSolutionDirectory();
                var futureImage = new ImageFromDockerfileBuilder()
                    .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                    .WithDockerfile("Dockerfile")
                    .WithCleanUp(true)
                    .Build();

                await futureImage.CreateAsync()
                    .ConfigureAwait(false);

                var randomGenerator = new RandomGenerator(DateTime.Now.Millisecond);

                var connection = Regex.Replace(connectionString, "(Server=\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3},\\d+;)", $"Server={sqlServerHost};");

                var HttpPort = randomGenerator.Next(2000, 9999);
                var myServiceContainer = new ContainerBuilder()
                  .WithName(Guid.NewGuid().ToString("D"))
                  .WithImage(futureImage.FullName)
                  .WithEnvironment("ConnectionStrings__OmsDbConnection", connection)                  
                  .WithEnvironment("Kestrel__EndPoints__Http__Url", $"http://+:{HttpPort}")
                  .WithPortBinding(HttpPort, true)
                  .WithNetwork(network)
                  .Build();

                await myServiceContainer.StartAsync()
                  .ConfigureAwait(false);

                var endPointService = myServiceContainer.IpAddress;

                var uriBuilder = new UriBuilder("http", myServiceContainer.Hostname, myServiceContainer.GetMappedPublicPort(HttpPort));

                var uri = uriBuilder.Uri;

                var httpClient = new HttpClient();
                httpClient.BaseAddress = uri;
                var orders = await httpClient.GetFromJsonAsync<List<Order>>("orders");
                orders.Should().BeEmpty();

                var addOrder = Builder<AddOrderDto>.CreateNew().Build();
                await httpClient.PostAsJsonAsync("orders", addOrder);

                orders = await httpClient.GetFromJsonAsync<List<Order>>("orders");
                orders.Should().ContainEquivalentOf(addOrder);

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
            }
            finally
            {
                if (msSqlContainer != null)
                {
                    await msSqlContainer.StopAsync();
                    await msSqlContainer.DisposeAsync();
                }
            }
        }
    }
}
