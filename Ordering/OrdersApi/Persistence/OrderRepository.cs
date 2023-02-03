using Microsoft.EntityFrameworkCore;
using OrdersApi.Models;

namespace OrdersApi.Persistence
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _orderContext;
        public OrderRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }
        public Order GetOrder(Guid id)
        {
            return _orderContext.Orders
                .Include("OrderDetails")
                .FirstOrDefault(x => x.OrderId == id);
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            return await _orderContext.Orders
                .Include("OrderDetails")
                .FirstOrDefaultAsync(x => x.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _orderContext.Orders.ToListAsync();        
        }

        public Task RegisterOrder(Order order)
        {
            //todo: is it good practice?
            _orderContext.Add(order);
            _orderContext.SaveChanges();

            return Task.FromResult(true);
        }

        public void UpdateOrder(Order order)
        {
            //todo: this vs Update
            _orderContext.Entry(order).State = EntityState.Modified;
            _orderContext.SaveChanges();
        }
    }
}
