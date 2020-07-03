using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WindowsAndJWTAuth.Dtos
{
    public class CloudflareKeysDto
    {
        [JsonPropertyName("keys")]
        public CloudflareJwk[] Keys { get; set; }
    }

    public class CloudflareJwk
    {
        [JsonPropertyName("e")]
        public string Exponent { get; set; }
        [JsonPropertyName("n")]
        public string Modulus { get; set; }
    }

}
