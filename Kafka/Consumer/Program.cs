using Confluent.Kafka;

namespace Consumer
{
    public class Program
    {
        private static string BootstrapServers { get; set; } = "localhost:9092";
        private static string Topic { get; set; } = "kafka-topic";
        private static string ClientId { get; set; } = "kafka-client-id";
        private static string GroupId { get; set; } = "kafka-group-id";

        public static void Main(string[] args)
        {
            ConsumeMessage();
        }

        public static void ConsumeMessage()
        {
            var configuration = new ConsumerConfig
            {
                BootstrapServers = BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                ClientId = ClientId,
                GroupId = GroupId,
                BrokerAddressFamily = BrokerAddressFamily.V4
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(configuration).Build())
            {
                consumer.Subscribe(Topic);

                try
                {
                    while (true)
                    {
                        var consumeMessage = consumer.Consume();

                        Console.WriteLine($"Consume message '{consumeMessage.Message.Value}' <- {consumeMessage.TopicPartitionOffset}");
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    consumer.Close();
                }
            }

            Console.ReadLine();
        }
    }
}