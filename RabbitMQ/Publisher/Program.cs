using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "RabbitMQ-Queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            // [Exchange Type] direct, fanout, topic, or headers
            channel.ExchangeDeclare(
                exchange: "Logs",
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            string message = "Sending a message using ASP.NET Core RabbitMQ";
            byte[] body;
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            do
            {
                body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: "RabbitMQ-Queue",
                    basicProperties: properties,
                    body: body);
                channel.BasicPublish("Logs", "Information.General", null, Encoding.UTF8.GetBytes($"[Information] {message}"));
                channel.BasicPublish("Logs", "Warning.Minor", null, Encoding.UTF8.GetBytes($"[Warning] {message}"));
                channel.BasicPublish("Logs", "Error.Important", null, Encoding.UTF8.GetBytes($"[Error] {message}"));

                Console.WriteLine($"Publisher: \t {message} [{DateTime.Now}]");
                Console.Write("Enter message: \t ");
            }
            while (!string.IsNullOrEmpty(message = Console.ReadLine() ?? throw new Exception("STOP")));
        }
    }
}
