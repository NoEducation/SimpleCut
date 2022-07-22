using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace SimpleCut.Services.Accounts
{
    public class PasswordHasherService : IPasswordHasherService
    {
        //TODO.DA dodaj pieprz
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

        public bool CompareHashWithPassword(string passwordHash, string password, string salt)
        {
            var target = GenerateHash(password, salt);
            return passwordHash == target;
        }
    }
}
