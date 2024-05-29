using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.Publisher
{
    public class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("RabbitMQ-Queue", false, false, false, null);
                string message = "creating a message using asp.net core rabbitmq long ngu 123";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", "Test", null, body);
                Console.WriteLine("sent message:{0}", message);
            }

            Console.ReadLine();
        }
    }
}
