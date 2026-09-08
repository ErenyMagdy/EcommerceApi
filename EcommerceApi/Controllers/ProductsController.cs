using EcommerceApi.Dtos;
using EcommerceApi.Features.Products.Commands.CreateProduct;
using EcommerceApi.Features.Products.Commands.DeleteProduct;
using EcommerceApi.Features.Products.Commands.UpdateProduct;
using EcommerceApi.Features.Products.Queries.GetProductById;
using EcommerceApi.Features.Products.Queries.GetProducts;
using EcommerceApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductsController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<Product>>> GetProducts([FromQuery] GetProductsQuery queryParameters, CancellationToken cancellationToken)
        {
           var result = await _mediator.Send(queryParameters, cancellationToken);
            return Ok(result);
        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken) {
            var query = new GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(query ,cancellationToken);
            return Ok(product);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _mediator.Send(command,  cancellationToken );
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command,cancellationToken);
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
