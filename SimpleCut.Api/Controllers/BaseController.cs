using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class BaseController : ControllerBase
    {
        private IDispatcher _dispatcher;

        public BaseController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<OperationResult<TResult>> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            var result = await _dispatcher.SendAsync(query, cancellationToken);

            if (!result.Success)
            {
                Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            }

            return result;
        }

        public async Task<OperationResult> DispatchAsync<TResult>(ICommand command, CancellationToken cancellationToken = default)
        {
            var result = await _dispatcher.SendAsync(command, cancellationToken);

            if (!result.Success)
            {
                Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            }

            return result;
        }
    }
}
