using MasstransitRabbitMQ.Consumer.API.Abstractions.Messages;
using MasstransitRabbitMQ.Contract.IntegrationEvents;
using MediatR;

namespace MasstransitRabbitMQ.Consumer.API.MessageBus.Consumers.Events
{
    public class SendSMSWhenReceivedSMSEventConsumer : Consumer<DomainEvent.SMSNotificationEvent>
    {
        public SendSMSWhenReceivedSMSEventConsumer(ISender sender) : base(sender)
        {
        }
    }
}
