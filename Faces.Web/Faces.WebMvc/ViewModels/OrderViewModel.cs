using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Faces.WebMvc.ViewModels
{
    public class OrderViewModel
    {
        [Display(Name = "Order Id")]
        public Guid OrderId { get; set; }

        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Image File")]
        public IFormFile File { get; set; }

        [Display(Name = "Picture Url")]
        public string PictureUrl { get; set; }

        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageString { get; set; }
    }
}
