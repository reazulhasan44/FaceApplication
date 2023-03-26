﻿using EmailService;
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
    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {

        private readonly IEmailSender _emailSender;

        public OrderProcessedEventConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
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
            #region emailsection

            string[] mailAdress = { result.UserEmail };
            await _emailSender.SendEmailAsync(new Message(mailAdress,
                                "your order " + result.OrderId, "From F&F", facesData));

            #endregion emailsection

            await context.Publish<IOrderDispatchedEvent>(new {
                context.Message.OrderId,
                DispatchDateTime = DateTime.UtcNow
            });

        }
    }
}
