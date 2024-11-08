namespace Authentication.Models
{
    public class GenerateTokenDTO
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}
