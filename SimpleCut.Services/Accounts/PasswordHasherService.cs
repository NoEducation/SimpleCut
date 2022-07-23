using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using SimpleCut.Common.Options;
using System.Text;

namespace SimpleCut.Services.Accounts
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly TokenOptions _tokenOptions;

        public PasswordHasherService(IOptions<TokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
        }

        public string GenerateHash(string password)
        {
            var hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(_tokenOptions.Salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hash);
        }

        public bool CompareHashWithPassword(string passwordHash, string password)
        {
            var target = GenerateHash(password);
            return passwordHash == target;
        }
    }
}
