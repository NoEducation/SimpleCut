using Dapper;
using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Logic.Accounts.Commands
{
    public class RevokeRefreshTokenCommandHandler : ICommandHandler<RevokeRefreshTokenCommand>
    {
        private readonly IDbContext _context;

        public RevokeRefreshTokenCommandHandler(IDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            await Validate(request, result);

            if (!result.Success)
                return result;

            var parameters = new { @userId = request.UserId, @refreshTokenKey = request.RefreshToken };

            await _context.Connection.ExecuteAsync(@"
                UPDATE public.RefresTokens 
                    SET Expires = now(),
                        Revoked = now()
                WHERE userId = @userId and token = @refreshTokenKey;
            ", parameters);

            return result;
        }

        private async Task Validate(RevokeRefreshTokenCommand command, OperationResult result)
        {
            var parameters = new
            {
                @userId = command.UserId,
                @refreshTokenKey = command.RefreshToken
            };

            var refresTokenExists = await this._context.Connection.QuerySingleAsync<bool>(@"
                  SELECT CASE
                        WHEN EXISTS(SELECT 1 FROM public.RefresTokens 
                                WHERE userId = @userId and token = @refreshTokenKey and isActive = true )
                            THEN 1
                       ELSE 0
                  END RefresTokenExists;
            ", parameters);

            if (!refresTokenExists)
                result.AddError("Refresh token does not exists", nameof(command.RefreshToken));
        }
    }
}
