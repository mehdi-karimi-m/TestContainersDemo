using Dapper;
using Microsoft.Data.SqlClient;

namespace OmsService.WebApi.Infrastructure
{
    public class Migration
    {
        private string _connectionString;
        private readonly ILogger<Migration> _logger;
        const string CreateOrderTableQuery = @"
                                                USE [OmsDb]

                                                IF OBJECT_ID('Orders', 'U') IS NULL
                                                BEGIN

	                                                SET ANSI_NULLS ON

	                                                SET QUOTED_IDENTIFIER ON

	                                                CREATE TABLE [dbo].[Orders](
		                                                [Id] [uniqueidentifier] NOT NULL,
		                                                [TraderPamCode] [varchar](50) NOT NULL,
		                                                [InstrumentIsin] [varchar](50) NOT NULL,
		                                                [Quantity] [int] NOT NULL,
		                                                [Price] [int] NOT NULL,
		                                                [OrderSide] [tinyint] NOT NULL,
	                                                 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
	                                                (
		                                                [Id] ASC
	                                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	                                                ) ON [PRIMARY]

                                                END
                                                ";
        const string CreateOmsDbDatabaseQuery = @"
                                                USE master;

                                                IF DB_ID('OmsDb') IS NULL
                                                BEGIN
                                                    CREATE DATABASE OmsDb ON
                                                    (NAME = OmsDb_dat,
                                                        FILENAME = '/var/opt/mssql/data/OmsDbDat.mdf',
                                                        SIZE = 10,
                                                        MAXSIZE = 50,
                                                        FILEGROWTH = 5)
                                                    LOG ON
                                                    (NAME = OmsDb_log,
                                                        FILENAME = '/var/opt/mssql/data/OmsDbLog.ldf',
                                                        SIZE = 5 MB,
                                                        MAXSIZE = 25 MB,
                                                        FILEGROWTH = 5 MB);
                                                END
                                                ";

        public Migration(string connectionString, ILogger<Migration> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task Migrate()
        {
            try
            {
                _logger.LogInformation("strarting create OmsDb database.");
                await CreateDatabase();
                _logger.LogInformation("OmsDb database created.");

                _logger.LogInformation("strarting create tables.");
                await CreateTables();
                _logger.LogInformation("Tables created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
        private async Task CreateDatabase()
        {
            using var db = new SqlConnection(_connectionString);
            await db.ExecuteAsync(CreateOmsDbDatabaseQuery);
        }
        private async Task CreateTables()
        {
            using var db = new SqlConnection(_connectionString);
            await db.ExecuteAsync(CreateOrderTableQuery);
        }
    }
}
