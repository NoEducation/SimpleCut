using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Logic.Account.Queries;

namespace SimpleCut.Api.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IDispatcher dispatcher) : base(dispatcher)
        {}

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<OperationResult<GetAccountTokenQueryResponse>> Login([FromBody] GetAccountTokenQuery query)
        {
            var response = await this.DispatchAsync(query);

            return response;
        }

        //[HttpPost("Register")]
        //[AllowAnonymous]
        //public async Task<OperationResult> Register(RegisterUserDto model)
        //{
        //    try
        //    {
        //        await this._accountService.Register(model);
        //    }
        //    catch (ValidationArchitectureException exception)
        //    {
        //        return BadRequest(exception.Errors.ToString());
        //    }

        //    return Ok();
        //}


        //[HttpGet("RefreshToken")]
        //[AllowAnonymous]
        //public async Task<ActionResult<string>> RefreshToken(int userId)
        //{
        //    string accessToken = null;
        //    var refreshToken = Request.Cookies["refreshToken"];

        //    try
        //    {
        //        accessToken = await this._accountService.GetAccessToken(refreshToken, userId);
        //    }
        //    catch (AuthorizationArchitectureException)
        //    {
        //        return Unauthorized();
        //    }

        //    return Ok(accessToken);
        //}

        //[HttpPost("RevokeRefreshToken")]
        //public async Task<ActionResult<string>> RevokeRefreshToken(string refreshToken)
        //{
        //    try
        //    {
        //        await this._accountService.RevokeRefreshToken(refreshToken,
        //            Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
        //    }
        //    catch (AuthorizationArchitectureException)
        //    {
        //        return BadRequest("Invalid token");
        //    }

        //    return Ok();
        //}

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7) // TODO.DA zwraca cały obiekt i ustwaic tu wartość
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
