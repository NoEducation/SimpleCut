namespace SimpleCut.Logic.Accounts.Queries
{
    public class GetAccountTokenQueryResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
