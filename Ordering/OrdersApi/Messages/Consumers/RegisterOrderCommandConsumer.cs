using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Newtonsoft.Json;
using OrdersApi.Models;
using OrdersApi.Persistence;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _iOrderRepository;
        private readonly IHttpClientFactory _iHttpClientFactory;
        public RegisterOrderCommandConsumer(IOrderRepository iOrderRepository, IHttpClientFactory iHttpClientFactory)
        {
            _iOrderRepository = iOrderRepository;
            _iHttpClientFactory = iHttpClientFactory;
        }

        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;

            if (result?.OrderId != null && result.PictureUrl != null &&
                result.UserEmail != null && result.ImageData != null)
            {
                SaveOrder(result);
                var client = _iHttpClientFactory.CreateClient();
                Tuple<List<byte[]>, Guid> orderDetailsData = await GetFacesFromFaceApiAsync(client, result.ImageData
                    , result.OrderId);
                var faces = orderDetailsData.Item1;
                var orderId = orderDetailsData.Item2;
                SaveOrderDetails(orderId, faces);

            }
        }

        // this method calls FacesApi
        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApiAsync(HttpClient client, byte[] imageData, Guid orderId)
        {
            var byteContent = new ByteArrayContent(imageData);
            Tuple<List<byte[]>, Guid> orderDetailsData = null;
            using (var response  = await client.PostAsync("http://localhost:6001/api/faces/" + orderId, byteContent))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                orderDetailsData = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
            }
            return orderDetailsData;
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

        private void SaveOrderDetails(Guid orderId, List<byte[]> faces)
        {
            var order = _iOrderRepository.GetOrderAsync(orderId).Result;
            if (order != null)
            {
                order.Status = Status.Processed;
                foreach (var face in faces)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        FaceData = face

                    };
                    order.OrderDetails.Add(orderDetail);
                }
                _iOrderRepository.UpdateOrder(order);
            }
        }
    }
}
