using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Persistence;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _iOrderRepository;
        public OrdersController(IOrderRepository iOrderRepository)
        {
              _iOrderRepository = iOrderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync()
        {
            var orders = await _iOrderRepository.GetOrdersAsync();
            if (orders == null) return BadRequest();

            return Ok(orders);
        }

        [HttpGet]
        [Route("{orderId}", Name = "GetOrderById")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var order = await _iOrderRepository.GetOrderAsync(Guid.Parse(orderId));
            if (order == null) return NotFound();

            return Ok(order);
        }

    }
}
