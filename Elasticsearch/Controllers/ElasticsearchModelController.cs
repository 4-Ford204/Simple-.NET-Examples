using Microsoft.AspNetCore.Mvc;
using Nest;

namespace Elasticsearch.Controllers
{
    [ApiController]
    [Route("elasticsearch/[controller]")]
    public class ElasticsearchModelController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ElasticsearchModelController(IElasticClient elasticClient, IWebHostEnvironment hostingEnvironment)
        {
            _elasticClient = elasticClient;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public string EnsureConnection() => "Elasticsearch - Elasticsearch Model";

        [HttpGet, Route("Index/{keyword}")]
        public async Task<IActionResult> Index(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var response = await _elasticClient.SearchAsync<ElasticsearchModel>(s => s
                    .Query(q => q
                        .QueryString(d => d
                            .Query($"*{keyword}*")
                        )
                    )
                    .Size(5000)
                );

                if (!response.IsValid)
                {
                    return BadRequest(response.OriginalException.Message ??
                        (response.ServerError?.Error?.ToString()));
                }

                var result = response.Documents.ToList();
                return Ok(result);
            }

            return BadRequest("Keyword is required");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ElasticsearchModel model)
        {
            try
            {
                var response = await _elasticClient.IndexDocumentAsync(model);

                if (!response.IsValid)
                {
                    throw new Exception(response.OriginalException.Message ??
                        (response.ServerError?.Error?.ToString()));
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ElasticsearchModel model)
        {
            try
            {
                var response = await _elasticClient.UpdateAsync<ElasticsearchModel>(model.Id,
                    x => x.Index("elasticsearchmodels").Doc(model));

                if (!response.IsValid)
                {
                    throw new Exception(response.OriginalException.Message ??
                        (response.ServerError?.Error?.ToString()));
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _elasticClient.DeleteAsync<ElasticsearchModel>(id,
                    x => x.Index("elasticsearchmodels"));

                if (!response.IsValid)
                {
                    throw new Exception(response.OriginalException.Message ??
                        (response.ServerError?.Error?.ToString()));
                }

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
