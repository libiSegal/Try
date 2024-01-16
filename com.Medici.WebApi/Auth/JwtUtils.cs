using com.Medici.WebApi.Objects;
using com.Medici.WebApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace com.Medici.WebApi.Auth
{
    public class JwtUtils
    {
        private readonly AuthSettings authSettings;

        public JwtUtils(AuthSettings authSettings)
        {
            this.authSettings = authSettings;
        }

        public string GenerateJwtToken(UserObject user)
        {
            var claims = new List<Claim>
            {
                new Claim("Username", user.Username!),
            };

            var key = Encoding.UTF8.GetBytes(authSettings!.Secret!);
            var token = new JwtSecurityToken(
                issuer: "Medici",
                audience: "Medici",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            var handler = new JwtSecurityTokenHandler();

            string writtenToken = handler.WriteToken(token);

            return writtenToken;
        }

        public JwtSecurityToken? ValidateJwtToken(string token)
        {
            if (token == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(authSettings!.Secret!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return (JwtSecurityToken)validatedToken;
            }
            catch
            {
                return null;
            }
        }
    }
}
