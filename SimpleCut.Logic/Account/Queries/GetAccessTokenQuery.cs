using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Account.Queries
{
    //TODO.DA It should be renamed
    public class GetAccessTokenQuery : IQuery<GetAccessTokenQueryResponse>
    {
        public string? Password { get; set; }

        public GetAccessTokenQuery()
        {}

        public GetAccessTokenQuery(string password)
        {
            Password = password;
        }
    }
}
