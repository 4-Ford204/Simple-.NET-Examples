namespace MasstransitRabbitMQ.Producer.API.DIs.Options
{
    public class MasstransitConfiguration
    {
        public string Host { get; set; }
        public string VHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string SMSQueueName { get; set; }
        public string EmailQueueName { get; set; }
    }
}
