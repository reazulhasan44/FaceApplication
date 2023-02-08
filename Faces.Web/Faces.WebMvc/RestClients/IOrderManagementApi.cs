using Faces.WebMvc.ViewModels;
using Refit;

namespace Faces.WebMvc.RestClients
{
    public interface IOrderManagementApi
    {
        // refit lib attribute
        [Get("/orders/")]
        Task<List<OrderViewModel>> GetOrders();

        [Get("/orders/{orderId}")]
        Task<OrderViewModel> GetOrderById(Guid orderId);
    }
}
