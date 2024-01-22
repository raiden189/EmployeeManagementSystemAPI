using EmployeeAPI.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeAPI.Helper
{
    public static class JWTAuthentication
    {
        public static AuthenticationOptions CreateScheme() => new AuthenticationOptions()
        {
            DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
            DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme,
            DefaultScheme = JwtBearerDefaults.AuthenticationScheme
        };

        public static TokenValidationParameters AddTokenValidation(WebApplicationBuilder builder) => new TokenValidationParameters() 
        {
           ValidateIssuer = true,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Audience"],
           ValidateAudience = true,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
           ValidateLifetime = false,
           ValidateIssuerSigningKey = true
        };

        public static SecurityTokenDescriptor TokenDescriptor(WebApplicationBuilder builder, User user)
        {
            var secureKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];
            var securityKey = new SymmetricSecurityKey(secureKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            
            return new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[] 
                {
                    //new Claim("Id", "1"),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, "raidengreat123@gmail.com"),
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.Now.AddHours(2),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = credentials
            };
        }
    }
}
