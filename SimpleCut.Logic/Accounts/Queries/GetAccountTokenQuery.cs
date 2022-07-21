using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Accounts.Queries
{
    // To powinnen być command ? / bo jednoczesnie zwracamy i udateujemy 
    // musimy to podzieli c na query i command
    public class GetAccountTokenQuery : IQuery<GetAccountTokenQueryResponse>
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
        public GetAccountTokenQuery(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public GetAccountTokenQuery()
        {}
    }
}
