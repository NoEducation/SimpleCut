using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using SimpleCut.Common.Dtos;
using SimpleCut.Common.Options;
using SimpleCut.Infrastructure.Cqrs;
using System.Text;

namespace SimpleCut.Logic.Account.Queries
{
    public class GetAccessTokenQueryHandler : IQueryHandler<GetAccessTokenQuery, GetAccessTokenQueryResponse>
    {
        private readonly TokenOptions _tokenOptions;
        public GetAccessTokenQueryHandler(IOptions<TokenOptions> options)
        {
            _tokenOptions = options.Value;
        }

        public Task<OperationResult<GetAccessTokenQueryResponse>> Handle(GetAccessTokenQuery request, CancellationToken cancellationToken)
        {
            var hash = GenerateHash(request.Password, _tokenOptions.Salt);

            var result = new OperationResult<GetAccessTokenQueryResponse>()
            {
                Result = new GetAccessTokenQueryResponse()
                {
                    PasswordHash = hash
                }
            };

            return Task.FromResult(result);
        }

        private string GenerateHash(string? password, string? salt)
        {
            var hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hash);
        }
    }
}
