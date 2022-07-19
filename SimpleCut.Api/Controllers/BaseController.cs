using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class BaseController : ControllerBase
    {
        protected IDispatcher Dispatcher { get; private set; }

        public BaseController(IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }
    }
}
