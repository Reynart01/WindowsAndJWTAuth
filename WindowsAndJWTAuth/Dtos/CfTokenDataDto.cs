using System.Text.Json.Serialization;

namespace WindowsAndJWTAuth.Dtos
{
    public class CfTokenDataDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        public long Expiration { get; set; }
    }
}
