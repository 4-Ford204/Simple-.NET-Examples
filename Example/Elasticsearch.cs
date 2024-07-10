using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class Elasticsearch
    {
        private ElasticsearchClient client = null;

        private const string USER_NAME = "elastic";
        private const string PASSWORD = "d6Gx4RspRVOUa0Uxxd-J";
        private const string CERTIFICATE = "3ade378aecd496de09cb80f71a007674490c6b154149b1183adaad73f0a69d72";

        private List<User> sampleUsers = new List<User>()
        {
            new User() { Id = 1, Name = "JavaScript" },
            new User() { Id = 2, Name = "SQL" },
            new User() { Id = 3, Name = "Python" },
            new User() { Id = 4, Name = "C#" },
            new User() { Id = 5, Name = "Rust" },
            new User() { Id = 6, Name = "Solidity" },
            new User() { Id = 7, Name = "Go" },
            new User() { Id = 8, Name = "Ruby" },
            new User() { Id = 9, Name = "React" },
            new User() { Id = 10, Name = "VueJS" }
        };

        public Elasticsearch()
        {
            var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200"))
                .CertificateFingerprint(CERTIFICATE)
                .Authentication(new BasicAuthentication(USER_NAME, PASSWORD))
                .ThrowExceptions()
                .PrettyJson()
                .DisableDirectStreaming();
            settings.DefaultIndex("user-index");
            client = new ElasticsearchClient(settings);
            //client = new ElasticsearchClient();
        }

        public async Task<string> IndexingDocument()
        {
            StringBuilder result = new StringBuilder();

            foreach (var user in sampleUsers)
            {
                var response = await client.IndexAsync(user, index: "user-index");

                if (response.IsValidResponse)
                    result.AppendLine($"Index document {user.ToString()} with id {response.Id}");
                else
                    result.AppendLine($"Error: {response}");
            }

            return result.ToString();
        }

        /// <summary>
        /// Truy vấn document có id = 1 (id được indexing trong Elasticsearch) của index = user-index
        /// </summary>
        /// <returns></returns>
        public async Task<User> GettingDocument()
        {
            var response = await client.GetAsync<User>(1, idx => idx.Index("user-index"));
            Console.WriteLine(response);

            return response.IsValidResponse ? response.Source : null;
        }

        public async Task<List<User>> SearchingAllDocument()
        {
            var response = await client.SearchAsync<User>("user-index");
            Console.WriteLine(response);

            return response.Documents.ToList();
        }

        public async Task<List<User>> SearchingDocument()
        {
            List<User> result = new List<User>();

            var request = new SearchRequest("user-index")
            {
                From = 0,
                Size = 10,
                Query = new TermQuery("name")
                {
                    Value = "javascript"
                }
            };
            var response = await client.SearchAsync<User>(request);

            //var response = await client.SearchAsync<User>(s => s
            //    .Index("user-index")
            //    .From(0)
            //    .Size(10)
            //    .Query(q => q.Term(t => t.Field(f => f.Name).Value("javascript"))));

            if (response.IsValidResponse)
                result = response.Documents.ToList();
            Console.WriteLine(response);

            return result;
        }

        public async Task<string> UpdatingDocument()
        {
            var user = new User()
            {
                Id = 1,
                Name = "JS"
            };

            var response = await client.UpdateAsync<User, User>("user-index", 1, u => u.Doc(user));
            Console.WriteLine(response);

            return response.IsValidResponse ? "Update Succeeded" : "Update Failed";
        }

        public async Task<string> DeletingDocument()
        {
            var response = await client.DeleteAsync("user-index", 1);
            Console.WriteLine(response);

            return response.IsValidResponse ? "Delete Succeeded" : "Delete Failed";
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => $"[Id: {Id}, Name: {Name}]";
    }
}
