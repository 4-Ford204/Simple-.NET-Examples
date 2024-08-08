using MassTransit;
using MasstransitRabbitMQ.Contract.Constants;
using MasstransitRabbitMQ.Contract.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;

namespace MasstransitRabbitMQ.Producer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ProducersController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost(Name = "publish-sms-notification")]
        public async Task<IActionResult> PublishSMSNotificationEvent()
        {
            await _publishEndpoint.Publish(new DomainEvent.SMSNotificationEvent()
            {
                Id = Guid.NewGuid(),
                Description = "SMS Decription",
                Name = "SMS Decription",
                TimeStamp = DateTime.Now,
                TransactionId = Guid.NewGuid(),
                Type = NotificationType.SMS
            });

            return Accepted();
        }
    }
}
