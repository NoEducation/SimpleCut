using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleCut.Common.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimpleCut.Infrastructure.Services.Accounts
{
    public class TokenService : ITokenService
    {
        private readonly TokenOptions _tokenOptions;

        public TokenService(IOptions<TokenOptions> tokenOpitons)
        {
            _tokenOptions = tokenOpitons.Value;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenTimeValid),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Secrete)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtHandler.CreateToken(descriptor);
            return jwtHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace(" - ", "");
        }

        //TODO.DA musze się jeszcze nad tym zastanowic
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
