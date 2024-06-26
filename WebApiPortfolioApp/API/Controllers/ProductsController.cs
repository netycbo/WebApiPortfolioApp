using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPortfolioApp.API.Request;

namespace WebApiPortfolioApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,User")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductSearchRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        [HttpGet("UpdateProductPrice")]
        public async Task<IActionResult> UpdateAndCompare ([FromQuery] UpdatePriceProduktRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("GetAllProductsFromDb")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsNameRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("FindAndAddProductsToNewsletter")]
        public async Task<IActionResult> SingUp([FromQuery] AddProductsToNewsLetterRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
