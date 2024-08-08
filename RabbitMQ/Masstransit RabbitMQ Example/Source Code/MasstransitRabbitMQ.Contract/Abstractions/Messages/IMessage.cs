using MassTransit;
using MediatR;

namespace MasstransitRabbitMQ.Contract.Abstractions.Messages
{
    [ExcludeFromTopology]
    public interface IMessage : IRequest
    {
        public Guid Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
