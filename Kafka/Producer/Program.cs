using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Producer
{
    public class Program
    {
        private static string BootstrapServers { get; set; } = "localhost:9092";
        private static string Topic { get; set; } = "kafka-topic";
        private static string ClientId { get; set; } = "kafka-client-id";

        public static void Main(string[] args)
        {
            GetKafka().Wait();
            ProduceMessage().Wait();
        }

        public static async Task GetKafka()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = BootstrapServers }).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(
                        [
                            new TopicSpecification {
                                Name = Topic,
                                ReplicationFactor = 1,
                                NumPartitions = 1
                            }
                        ]
                    );
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"{e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
            }
        }

        public static async Task ProduceMessage()
        {
            var configuration = new ProducerConfig
            {
                BootstrapServers = BootstrapServers,
                ClientId = ClientId,
                BrokerAddressFamily = BrokerAddressFamily.V4
            };

            using (var producer = new ProducerBuilder<Null, string>(configuration).Build())
            {
                string? input;
                Console.WriteLine("Enter message: ");

                while (!String.IsNullOrEmpty(input = Console.ReadLine()))
                {
                    var message = new Message<Null, string>
                    {
                        Value = input
                    };
                    var produceMessage = await producer.ProduceAsync(Topic, message);

                    Console.WriteLine($"Produce message '{message}' -> {produceMessage.TopicPartitionOffset}");
                }
            }

            Console.ReadLine();
        }
    }
}