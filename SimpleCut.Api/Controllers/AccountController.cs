using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleCut.Common.Dtos;
using SimpleCut.Common.Options;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Logic.Accounts.Commands;
using SimpleCut.Logic.Accounts.Queries;

namespace SimpleCut.Api.Controllers
{
    public class AccountController : BaseController
    {
        private readonly TokenOptions _tokenOptions;

        public AccountController(IDispatcher dispatcher,
            IHttpContextAccessor httpContextAccessor,
            IOptions<TokenOptions> tokenOptions) : base(dispatcher, httpContextAccessor)
        {
            _tokenOptions = tokenOptions.Value;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<OperationResult<GetAccountTokenQueryResponse>> Login([FromBody] GetAccountTokenQuery query)
        {
            var response = await this.DispatchAsync(query);

            if (!response.Success)
                return response;

            await this.DispatchAsync(new CreateRefreshTokenCommand()
            { 
                RefreshTokenKey = response.Result.RefreshToken,
                UserId = response.Result.UserId
            });

            SetTokenCookie(response.Result.RefreshToken);
            response.Result.RefreshToken = null;

            return response;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<OperationResult> Register(CreateUserCommand command)
        {
            var result = await this.DispatchAsync(command);

            return result;
        }


        [HttpGet("RefreshToken")]
        [AllowAnonymous]
        public async Task<OperationResult<GetAccessTokenBasedOnRefreshTokenQueryResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await DispatchAsync(
                new GetAccessTokenBasedOnRefreshTokenQuery()
                {
                    UserId = CurrentUserId,
                    RefreshToken = refreshToken
                });

            return result;
        }

        [HttpPost("RevokeRefreshToken")]
        public async Task<OperationResult> RevokeRefreshToken([FromQuery] string refreshToken)
        {
            var result = await this.DispatchAsync(new RevokeRefreshTokenCommand()
            {
                RefreshToken = refreshToken,
                UserId = CurrentUserId
            });

            return result;
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenTimeValid)
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
