namespace SimpleCut.Services.Accounts
{
    public interface IPasswordHasherService
    {
        public string GenerateHash(string password, string salt);
        public bool CompareHashWithPassword(string passwordHash, string password, string salt);
    }
}
