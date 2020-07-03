using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtSecurityTokenHandler = System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;

namespace WindowsAndJWTAuth.Services
{
    public class JwtAuthenticationService
    {
        public string GetToken(string userEmail, long expiration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("asdjk89asdjaskldj09asdjaskldj0");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.Name, userEmail)
                }),
                Expires = DateTimeOffset.FromUnixTimeSeconds(expiration).LocalDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token =  tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}