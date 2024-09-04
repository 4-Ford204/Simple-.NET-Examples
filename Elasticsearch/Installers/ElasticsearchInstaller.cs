using Elasticsearch.Configuration;
using Nest;

namespace Elasticsearch.Installers
{
    public class ElasticsearchInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var elasticsearchConfiguration = new ElasticsearchConfiguration();
            configuration.GetSection("ElasticsearchConfiguration").Bind(elasticsearchConfiguration);

            var settings = new ConnectionSettings(new Uri(elasticsearchConfiguration.BaseURL ?? ""))
                .PrettyJson()
                .CertificateFingerprint(elasticsearchConfiguration.Certificate)
                .BasicAuthentication(elasticsearchConfiguration.UserName, elasticsearchConfiguration.Password)
                .DefaultIndex(elasticsearchConfiguration.DefaultIndex);
            settings.EnableApiVersioningHeader();
            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
            CreateIndex(client, elasticsearchConfiguration.DefaultIndex);
        }

        private void AddDefaultMappings(ConnectionSettings settings)
        {
            //settings.DefaultMappingFor<ElasticsearchModel>(model => model.Ignore(x => x.Description).Ignore(x => x.User));
        }

        private void CreateIndex(ElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName, index => index.Map<ElasticsearchModel>(m => m.AutoMap()));
        }
    }
}
