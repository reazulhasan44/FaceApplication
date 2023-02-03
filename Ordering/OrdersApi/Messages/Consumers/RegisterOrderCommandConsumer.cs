using MassTransit;
using Messaging.InterfacesConstants.Commands;
using OrdersApi.Models;
using OrdersApi.Persistence;
using System.Net.Http;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _iOrderRepository;
        public RegisterOrderCommandConsumer(IOrderRepository iOrderRepository)
        {
            _iOrderRepository = iOrderRepository;
        }

        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;

            if (result?.OrderId != null && result.PictureUrl != null &&
                result.UserEmail != null && result.ImageData != null)
            {
                SaveOrder(result);
            }
        }

        private void SaveOrder(IRegisterOrderCommand result)
        {
            Order order = new Order
            {
                OrderId = result.OrderId,
                UserEmail = result.UserEmail,
                Status = Status.Registered,
                PictureUrl = result.PictureUrl,
                ImageData = result.ImageData

            };
            _iOrderRepository.RegisterOrder(order);
        }
    }
}
