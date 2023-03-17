using MassTransit;
using Messaging.InterfacesConstants.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Consumers
{
    internal class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var result = context.Message;
            var facesData = result.Faces;
            if (facesData.Count < 1)
            {
                await Console.Out.WriteLineAsync($"No faces detected");
            }
            else 
            {
                int j = 0;
                foreach (var face in facesData)
                {
                    MemoryStream ms = new MemoryStream(face);
                    var image = Image.FromStream(ms);
                    image.Save(rootFolder + "/Images/face" + j + ".jpg", format: ImageFormat.Jpeg);
                    j++;
                }
            }
            /* email section 
                code to write
             email section */

            await context.Publish<IOrderDispatchedEvent>(new {
                context.Message.OrderId,
                DispatchDateTime = DateTime.UtcNow
            });

        }
    }
}
