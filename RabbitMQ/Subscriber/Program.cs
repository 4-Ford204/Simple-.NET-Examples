using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Subscriber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            //channel.QueueDeclare("RabbitMQ-Queue", false, false, false, null);
            //channel.ExchangeDeclare("Logs", "topic", false, false, null);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, "Logs", "Information.General");
            channel.QueueBind(queueName, "Logs", "*.Minor");
            channel.QueueBind(queueName, "Logs", "Error.*");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var messageBody = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(messageBody);
                Console.WriteLine($"Subscriber: {message}");
            };
            channel.BasicConsume("RabbitMQ-Queue", true, consumer);
            channel.BasicConsume(queueName, true, consumer);

            while (true) { }
        }
    }
}
