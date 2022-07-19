namespace SimpleCut.Logic.Account.Queries
{
    public class GetAccessTokenQueryResponse 
    {
        public string? PasswordHash { get; set; }

        public GetAccessTokenQueryResponse()
        {}

        public GetAccessTokenQueryResponse(string? passwordHash)
        {
            PasswordHash = passwordHash;
        }
    }
}
