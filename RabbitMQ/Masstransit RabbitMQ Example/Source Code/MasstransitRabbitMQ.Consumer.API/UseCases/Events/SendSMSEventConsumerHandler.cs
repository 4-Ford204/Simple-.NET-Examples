using MasstransitRabbitMQ.Contract.IntegrationEvents;
using MediatR;

namespace MasstransitRabbitMQ.Consumer.API.UseCases.Events
{
    public class SendSMSEventConsumerHandler : IRequestHandler<DomainEvent.SMSNotificationEvent>
    {
        private readonly ILogger<SendSMSEventConsumerHandler> _logger;

        public SendSMSEventConsumerHandler(ILogger<SendSMSEventConsumerHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(DomainEvent.SMSNotificationEvent request, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            _logger.LogInformation("Message received: {Message}", request);
        }
    }
}
