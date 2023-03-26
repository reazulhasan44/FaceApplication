using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Messaging.InterfacesConstants.Events;
using OrdersApi.Models;
using OrdersApi.Persistence;

namespace OrdersApi.Messages.Consumers
{
    public class OrderDispatchedEventConsumer : IConsumer<IOrderDispatchedEvent>
    {
        private readonly IOrderRepository _iOrderRepository;
        public OrderDispatchedEventConsumer(IOrderRepository iOrderRepository)
        {
            _iOrderRepository = iOrderRepository;
        }

        public Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var message = context.Message;
            Guid orderId = message.OrderId;
            UpdateDatabase(orderId);
            return Task.CompletedTask;
        }

        private void UpdateDatabase(Guid orderId)
        {
            var order = _iOrderRepository.GetOrder(orderId);
            if (order != null)
            {
                order.Status = Status.Sent;
                _iOrderRepository.UpdateOrder(order);
            }
        }
    }
}
