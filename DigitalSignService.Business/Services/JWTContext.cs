using DigitalSignService.Business.IServices;
using DigitalSignService.DAL.DTOs.Requests.Sign;
using DigitalSignService.DAL.DTOs.Responses.SignDTOs;
using DigitalSignService.DAL.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DigitalSignService.Business.Services
{
    public class JWTContext : IJWTContext
    {
        private readonly AuthSetting _authSetting;

        public JWTContext(IOptions<AuthSetting> options)
        {
            _authSetting = options.Value;
        }

        // NEED TO HANDLE: Case other provider
        public VTGenTokenRes GenerateToken(VTGenTokenReq request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSetting.SecretKeySign);
            List<Claim> tokenClaims = new List<Claim>()
            {
                new Claim("provider", "viettel"),
                new Claim("client_id", request.ClientId),
                new Claim("client_secret", request.ClientSecret),
                new Claim("grant_type", request.GrantType)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            tokenDescriptor.Expires = DateTime.UtcNow.AddHours(1);

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            return new VTGenTokenRes
            {
                AccessToken = accessToken,
                ExpiresIn = 3600000,
                ConsentedOn = utcNow.ToUnixTimeSeconds()
            };
        }
    }
}
