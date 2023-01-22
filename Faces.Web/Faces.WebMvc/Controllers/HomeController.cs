using Faces.WebMvc.Models;
using Faces.WebMvc.ViewModels;
using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Faces.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _iBusControl;

        public HomeController(ILogger<HomeController> logger, IBusControl iBusControl)
        {
            _logger = logger;
            _iBusControl = iBusControl;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel orderViewModel)
        {
            MemoryStream ms = new MemoryStream();
            using (var uploadedFile = orderViewModel.File.OpenReadStream())
            {
                await uploadedFile.CopyToAsync(ms);
            }
            orderViewModel.ImageData = ms.ToArray();
            orderViewModel.PictureUrl = orderViewModel.File.FileName;
            orderViewModel.OrderId = Guid.NewGuid();
            var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMqUri}" + $"/{RabbitMqMassTransitConstants.RegisterOrderCommandQueue}");
            var endPoint = await _iBusControl.GetSendEndpoint(sendToUri);
            await endPoint.Send<IRegisterOrderCommand>(
                new
                {
                    orderViewModel.OrderId,
                    orderViewModel.UserEmail,
                    orderViewModel.ImageData,
                    orderViewModel.PictureUrl
                });
            ViewData["OrderId"] = orderViewModel.OrderId;
            return View("Thanks");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}