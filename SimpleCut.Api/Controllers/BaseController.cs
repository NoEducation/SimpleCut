using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Cqrs;
using System.Security.Claims;

namespace SimpleCut.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class BaseController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController(IDispatcher dispatcher, IHttpContextAccessor httpContextAccessor)
        {
            _dispatcher = dispatcher;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<OperationResult<TResult>> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            var result = await _dispatcher.SendAsync(query, cancellationToken);

            if (!result.Success)
            {
                Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            }

            return result;
        }

        protected async Task<OperationResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            var result = await _dispatcher.SendAsync(command, cancellationToken);

            if (!result.Success)
            {
                Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            }

            return result;
        }

        protected int CurrentUserId => Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

    }
}
