using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPortfolioApp.API.Request;

namespace WebApiPortfolioApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HealthCheckController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> HealthCheck([FromQuery] HealthCheckRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
    
}
