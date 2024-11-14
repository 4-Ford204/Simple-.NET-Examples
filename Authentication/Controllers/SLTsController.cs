using Authentication.Repositories.Interfaces.SLTs;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("authentication/[controller]")]
    public class SLTsController : ControllerBase
    {
        private readonly ILogger<SLTsController> _logger;
        private readonly ITransient _firstTransient;
        private readonly ITransient _secondTransient;
        private readonly IScoped _firstScoped;
        private readonly IScoped _secondScoped;
        private readonly ISingleton _firstSingleton;
        private readonly ISingleton _secondSingleton;

        public SLTsController(ILogger<SLTsController> logger,
            ITransient firstTransient,
            ITransient secondTransient,
            IScoped firstScoped,
            IScoped secondScoped,
            ISingleton firstSingleton,
            ISingleton secondSingleton)
        {
            _logger = logger;
            _firstTransient = firstTransient;
            _secondTransient = secondTransient;
            _firstScoped = firstScoped;
            _secondScoped = secondScoped;
            _firstSingleton = firstSingleton;
            _secondSingleton = secondSingleton;
        }

        [HttpGet("SLT")]
        public IActionResult SLT()
        {
            return Ok(new
            {
                FirstTransient = _firstTransient.GenerateID(),
                SecondTransient = _secondTransient.GenerateID(),
                FirstScoped = _firstScoped.GenerateID(),
                SecondScoped = _secondScoped.GenerateID(),
                FirstSingleton = _firstSingleton.GenerateID(),
                SecondSingleton = _secondSingleton.GenerateID()
            });
        }
    }
}
