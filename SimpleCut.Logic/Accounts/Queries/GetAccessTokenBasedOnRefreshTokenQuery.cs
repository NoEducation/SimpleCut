using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Accounts.Queries
{
    public class GetAccessTokenBasedOnRefreshTokenQuery : IQuery<GetAccessTokenBasedOnRefreshTokenQueryResponse>
    {
        public int UserId { get; set; }
        public string? RefreshToken { get; set; }
    }
}
