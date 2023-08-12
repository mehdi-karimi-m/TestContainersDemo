namespace OmsService.WebApi.Model
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task AddAsync(Order order);
        Task DeleteAsync(Guid id);
    }
}
