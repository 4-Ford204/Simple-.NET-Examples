using Google.Protobuf.WellKnownTypes;
using gRPC.Protos;
using Grpc.Core;

namespace gRPC.Services
{
    public class MessageService : Message.MessageBase
    {
        public override Task<MessageResponse> SendMessage(MessageRequest request, ServerCallContext context)
        {
            var response = new MessageResponse
            {
                Message = $"Received message: {request.Message}",
                Date = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            return Task.FromResult(response);
        }
    }
}
