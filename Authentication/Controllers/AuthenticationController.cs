using Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("authentication/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpPost("GenerateJWTToken")]
        public IActionResult GenerateJWTToken([FromBody] GenerateTokenDTO generateTokenDTO)
        {
            if (ModelState.IsValid)
            {
                var defaultGenerateToken = new GenerateTokenDTO()
                {
                    Name = Environment.GetEnvironmentVariable("DEFAULT_NAME") ?? "",
                    Password = Environment.GetEnvironmentVariable("DEFAULT_PASSWORD") ?? ""
                };

                if (generateTokenDTO.Name.Equals(defaultGenerateToken.Name) &&
                    generateTokenDTO.Password.Equals(defaultGenerateToken.Password))
                {
                    var time = DateTime.UtcNow;
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(
                        [
                            new Claim(JwtRegisteredClaimNames.Name, generateTokenDTO.Name),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, time.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                        ]),
                        NotBefore = time,
                        Expires = time.Add(TimeSpan.FromSeconds(int.Parse(Configuration["JWTConfiguration:Expires"] ?? "300"))),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTConfiguration:Key"] ?? "")), SecurityAlgorithms.HmacSha256)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    return Ok(new { Token = tokenHandler.WriteToken(token) });
                }
            }

            return BadRequest();
        }
    }
}
