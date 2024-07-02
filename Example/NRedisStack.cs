using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Example
{
    public enum RedisType
    {
        Single = 0,
        Cluster = 1,
        Muxer = 2
    }

    public class NRedisStack
    {
        private ConnectionMultiplexer redis;
        private IDatabase database;
        ConfigurationOptions options;

        public NRedisStack(int type = 0)
        {
            switch (type)
            {
                case (int)RedisType.Cluster:
                    options = new ConfigurationOptions()
                    {
                        EndPoints =
                        {
                            { "localhost", 16379 },
                            { "localhost", 16380 }
                        }
                    };
                    redis = ConnectionMultiplexer.Connect(options);
                    break;
                case (int)RedisType.Muxer:
                    options = new ConfigurationOptions()
                    {
                        EndPoints = { { "my-redis.cloud.redislabs.com", 6379 } },
                        User = "default",
                        Password = "secret",
                        Ssl = true,
                        SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };

                    options.CertificateSelection += delegate
                    {
                        return new X509Certificate2("redis.pfx", "redis.pfx secret");
                    };

                    options.CertificateValidation += ValidateServerCertificate;

                    redis = ConnectionMultiplexer.Connect(options);
                    break;
                default:
                    redis = ConnectionMultiplexer.Connect("localhost");
                    break;
            }

            database = redis.GetDatabase();
        }

        public void Example()
        {
            // Command Interfaces
            IBloomCommands bf = database.BF();
            ICuckooCommands cf = database.CF();
            ICmsCommands cms = database.CMS();
            IGraphCommands graph = database.GRAPH();
            ITopKCommands topk = database.TOPK();
            ITdigestCommands tdigest = database.TDIGEST();
            ISearchCommands ft = database.FT();
            IJsonCommands json = database.JSON();
            ITimeSeriesCommands ts = database.TS();

            // Store and retrieve a simple string
            //database.StringSet("foo", "bar");
            //Console.WriteLine(database.StringGet("foo"));

            // Store and retrieve a HashMap
            var hash = new HashEntry[]
            {
                new HashEntry("name", "John"),
                new HashEntry("surname", "Smith"),
                new HashEntry("company", "Redis"),
                new HashEntry("age", "29")
            };
            database.HashSet("user-session:123", hash);
            var hashFields = database.HashGetAll("user-session:123");
            Console.WriteLine(string.Join("; ", hashFields));
        }

        /// <summary>
        /// Indexing and querying JSON documents
        /// </summary>
        public void IndexAndQuery()
        {
            // Get reference to database for search and JSON commands
            var ft = database.FT();
            var json = database.JSON();
            var user1 = new
            {
                name = "Paul John",
                email = "paul.john@example.com",
                age = 42,
                city = "London"
            };
            var user2 = new
            {
                name = "Eden Zamir",
                email = "eden.zamir@example.com",
                age = 29,
                city = "Tel Aviv"
            };
            var user3 = new
            {
                name = "Paul Zamir",
                email = "paul.zamir@example.com",
                age = 35,
                city = "Tel Aviv"
            };
            var schema = new Schema()
                .AddTextField(new FieldName("$.name", "name"))
                .AddTextField(new FieldName("$.city", "city"))
                .AddNumericField(new FieldName("$.age", "age"));

            ft.Create(
                "idx:users",
                new FTCreateParams().On(IndexDataType.JSON).Prefix("user:"),
                schema);

            json.Set("user:1", "$", user1);
            json.Set("user:2", "$", user2);
            json.Set("user:3", "$", user3);

            var res = ft.Search("idx:users", new Query("Paul @age:[30 40]"))
                .Documents
                .Select(x => x["json"]);
            Console.WriteLine(string.Join("\n", res));

            var res_cities = ft
                .Search("idx:users", new Query("Paul").ReturnFields(new FieldName("$.city", "city")))
                .Documents
                .Select(x => x["city"]);
            Console.WriteLine(string.Join(", ", res_cities));

            var request = new AggregationRequest("*").GroupBy("@city", Reducers.Count().As("count"));
            var result = ft.Aggregate("idx:users", request);
            for (int i = 0; i < result.TotalResults; i++)
            {
                var row = result.GetRow(i);
                Console.WriteLine($"{row["city"]} - {row["count"]}");
            }
        }

        private bool ValidateServerCertificate(
            object sender,
            X509Certificate? certificate,
            X509Chain? chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (certificate == null)
            {
                return false;
            }

            var ca = new X509Certificate2("redis_ca.pem");
            bool verdict = (certificate.Issuer == ca.Subject);
            if (verdict)
            {
                return true;
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }
    }
}
