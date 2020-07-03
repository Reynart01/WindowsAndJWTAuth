using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using WindowsAndJWTAuth.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace WindowsAndJWTAuth.SecurityTokenHandlers
{
    public class CfJwtSecurityTokenHandler: JwtSecurityTokenHandler
    {
        private string _stsUrl;
        public CfJwtSecurityTokenHandler(string stsUrl)
        {
            _stsUrl = stsUrl;
        }
        protected override JwtSecurityToken ValidateSignature(string token, TokenValidationParameters validationParameters)
        {
            CloudflareJwk[] cloudflareJwk = GetKeysFromCloudflare().GetAwaiter().GetResult();

            JwtSecurityToken result = null;

            foreach (var key in cloudflareJwk)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(
                    new RSAParameters()
                    {
                        Modulus = FromBase64Url(key.Modulus),
                        Exponent = FromBase64Url(key.Exponent)
                    });
                validationParameters.IssuerSigningKey = new RsaSecurityKey(rsa);

                try
                {
                    result = base.ValidateSignature(token, validationParameters);
                    break;
                }
                catch (SecurityTokenSignatureKeyNotFoundException e)
                {
                    //skipped
                }
            }

            return result;
        }

        private async Task<CloudflareJwk[]> GetKeysFromCloudflare()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{_stsUrl}/cdn-cgi/access/certs");
            if (!response.IsSuccessStatusCode)
            {

            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var cloudflareKeys = await JsonSerializer.DeserializeAsync<CloudflareKeysDto>(contentStream);

            return cloudflareKeys.Keys;
        }

        private byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url
                : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded
                .Replace("_", "/")
                .Replace("-", "+");

            return Convert.FromBase64String(base64);
        }
        static string key0 = "yaW9zouYDG-Y62J3AqDJadIu68y_8XwKMsEBLtxf-GrHs-DBthaJRwks1MwWfSo_XnzKbpEe5RRYFfQLtjz8sKXTZnwCs79ZSQ95mw6k1mrC1uFykqHZk2XjBYAm6aDmkPw2ZFtXCkqZP8k1hHMhamBSoobiCA6SPYZE8KBtDZB18HeEnAkJMPpBa7hozKUKyEnXstKbYA7C2vM1nN8ufWg23gJMtG8GbuZXupuxbFiQYv0He3w31Ybep9MXdV6DGdK4t-1MT6xY2yKI-2h2Oxs2PVqfQpmg6zFHtdvyZETDEyGx-cq-iE1nt65tPAWKop_kllcZ1l--toXJQjc3xQ";
        static string key1 = "q1QdhpTwtT7S8FOc6ZP4tN5mggdCDgYKp5PzbiXCJmLzTO8d2hwJKuEN0NooYKBMjVb3ZWfkjaaz-BvGZf2AxiXgqniMRtJbvRKWEqIqp4fApUtB9DxFIFwaneRryUSXjy4vWpweeIOnJBfYLwXYHbwXBlbdmP0wOCAgNzzhdLblTgw7dzgPEJ_J3dilBnocSYfzTNac2N1qSuay685OijqGRlw7-Y7E9zL5xccDjpwePIxTmdWKE4uHU2d1Xbk8CJ49Hvn1Fg_wkjcDoampb1jS0XHQnIuUP3wp6d_hUuPCmV_uHXZfunRqRLC6RdRH85cLTeVcqWqqu_588_ef1w";

 

    }
}
