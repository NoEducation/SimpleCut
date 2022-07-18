using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SimpleCut.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        protected IMediator Mediator { get; private set; }

        public BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
