using Testcontainers.MsSql;

namespace SimpleMsSqlExample
{
    public class CreateASqlServerContainerTests
    {
        [Fact]
        public async Task Test1()
        {
            MsSqlContainer? msSqlContainer = null;
            try
            {
                msSqlContainer = new MsSqlBuilder()
                                      .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
                                      .Build();
                msSqlContainer.Starting += MsSqlContainer_Starting;
                msSqlContainer.Started += MsSqlContainer_Started;

                await msSqlContainer.StartAsync();

                var connectionString = msSqlContainer.GetConnectionString();
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

        private void MsSqlContainer_Started(object? sender, EventArgs e)
        {
            
        }

        private void MsSqlContainer_Starting(object? sender, EventArgs e)
        {
            
        }
    }
}