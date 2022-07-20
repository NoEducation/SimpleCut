using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace SimpleCut.Infrastructure.Services.Accounts
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string GenerateHash(string password, string salt)
        {
            var hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hash);
        }

        public bool CompareHash(string passwordHash, string password, string salt)
        {
            var target = GenerateHash(password, salt);
            return passwordHash == target;
        }
    }
}
