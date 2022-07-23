namespace SimpleCut.Services.Accounts
{
    public interface IPasswordHasherService
    {
        public string GenerateHash(string password);
        public bool CompareHashWithPassword(string passwordHash, string password);
    }
}
