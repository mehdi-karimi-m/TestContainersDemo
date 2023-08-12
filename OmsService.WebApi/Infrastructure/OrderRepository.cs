using Dapper;
using Microsoft.Data.SqlClient;
using OmsService.WebApi.Model;

namespace OmsService.WebApi.Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private const string InsertOrderQuery = @"
                                                INSERT INTO [OmsDb].[dbo].[Orders]
                                                           ([Id]
                                                           ,[TraderPamCode]
                                                           ,[InstrumentIsin]
                                                           ,[Quantity]
                                                           ,[Price]
                                                           ,[OrderSide])
                                                VALUES
                                                           (@Id
                                                           ,@TraderPamCode
                                                           ,@InstrumentIsin
                                                           ,@Quantity
                                                           ,@Price
                                                           ,@OrderSide)
                                                ";
        private const string SelectAllOrdersQuery = "SELECT [Id], [TraderPamCode], [InstrumentIsin], [Quantity], [Price], [OrderSide] FROM [OmsDb].[dbo].[Orders]";
        private const string SelectOrderByIdQuery = "SELECT [Id], [TraderPamCode], [InstrumentIsin], [Quantity], [Price], [OrderSide] FROM [OmsDb].[dbo].[Orders] where Id = @Id";
        private const string DeleteAnOrderByIdQuery = "delete from [OmsDb].[dbo].[Orders] where Id = @Id";

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(Order order)
        {
            using var db = new SqlConnection(_connectionString);
            await db.ExecuteAsync(InsertOrderQuery, new
            {
                order.Id,
                order.TraderPamCode,
                order.InstrumentIsin,
                order.Quantity,
                order.Price,
                order.OrderSide
            });

        }

        public async Task DeleteAsync(Guid id)
        {
            using var db = new SqlConnection(_connectionString);
            await db.ExecuteAsync(DeleteAnOrderByIdQuery, new { Id = id });
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using var dbo = new SqlConnection(_connectionString);
            return await dbo.QueryAsync<Order>(SelectAllOrdersQuery);
        }

        public async Task<Order> GetAsync(Guid id)
        {
            using var dbo = new SqlConnection(_connectionString);
            return await dbo.QueryFirstOrDefaultAsync<Order>(SelectOrderByIdQuery, new { Id = id });
        }
    }
}
