using Dapper;
using Microsoft.Extensions.Options;
using SimpleCut.Common.Dtos;
using SimpleCut.Common.Options;
using SimpleCut.Domain.Accounts;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Accounts.Commands
{
    internal class CreateRefreshTokenCommandHandler : ICommandHandler<CreateRefreshTokenCommand>
    {
        private readonly IDbContext _context;
        private readonly TokenOptions _tokenOptions;

        public CreateRefreshTokenCommandHandler(IDbContext dbContext, IOptions<TokenOptions> tokenOptions)
        {
            _context = dbContext;
            _tokenOptions = tokenOptions.Value;
        }

        public async Task<OperationResult> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var refreshToken = CreateRefreshToken(request.UserId, request.RefreshTokenKey);

            var refreshTokens = await this._context.Connection.QueryAsync<RefreshToken>(@"
                SELECT refreshtokenid,
                    userid, 
                    token, 
                    expiresdate, 
                    createddate, 
                    revokeddate,
                    replacedbytoken
                FROM public.refreshtokens
                WHERE userid = @userId
            ", new { @userId = request.UserId });

            if (refreshTokens?.Any() == true)
            {
                var lastAvailableToken = refreshTokens.SingleOrDefault(x => x.IsActive);

                if (lastAvailableToken != null)
                {
                    lastAvailableToken.RevokedDate = DateTime.Now;
                    lastAvailableToken.ReplacedByToken = request.RefreshTokenKey;

                    await this._context.Connection.ExecuteAsync(@"
                        UPDATE public.refreshtokens
                            SET RevokedDate = NOW(),
                                ReplacedByToken = @refreshTokenKey
                        WHERE refreshtokenid = @refreshtokenid
                    ", new { @refreshTokenKey = request.RefreshTokenKey, @refreshtokenid = lastAvailableToken.RefreshTokenId });
                }
            }

            var parameters = new
            {
                @userid = refreshToken.UserId,
                @token = refreshToken.Token,
                @expiresdate = refreshToken.Token,
                @createddate = refreshToken.CreatedDate
            };

            await this._context.Connection.ExecuteAsync(@"
                INSERT INTO public.refreshtokens(
                    userid, token, expiresdate, createddate, revokeddate, replacedbytoken)
                VALUES (@userid, @token, @expiresdate, @createddate, null, null);
            ", parameters);

            return result;
        }

        private RefreshToken CreateRefreshToken(int userId, string refreshTokenKey)
        {
            var entity = new RefreshToken()
            {
                CreatedDate = DateTime.Now,
                ExpiresDate = DateTime.Now.AddDays(_tokenOptions.RefreshTokenTimeValid),
                Token = refreshTokenKey,
                UserId = userId,
            };

            return entity;
        }
    }
}
