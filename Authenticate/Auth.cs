using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace AWSDeployDemo.Authenticate
{
    public class  Auth : IJwtAuth
    {
        private const string  _username = "user1";
        private const string  _password = "user1";
        private readonly string  _key = "";
        
        public Auth(IConfiguration configuration)
        {
            _key = configuration["TokenKey"];
        }

        string IJwtAuth.Authenticate(string username, string password)
        {
            if(username!=_username || password != _password)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);

            var tokenDescriptior = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim(ClaimTypes.Name,username),
                        new Claim("MyRole", "Boss")
                    }
                ),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptior);
            return tokenHandler.WriteToken(token);
        }

    }
}
