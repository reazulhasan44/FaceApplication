using OrdersApi.Models;

namespace OrdersApi.Persistence
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderAsync(Guid id);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task RegisterOrder(Order order);


        // not asynchronous bc of some bugs?
        Order GetOrder(Guid id);
        void UpdateOrder(Order order);
    }
}
