using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Logic.Account.Queries
{
    public class GetAccountTokenQuery : IRequest<OperationResult<GetAccountTokenQueryResponse>>
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
