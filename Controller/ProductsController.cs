using CatalogApi.Application.Services;
using CatalogApi.Contracts.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        // GET /api/v1/products?category=Electronics&inStock=true&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQuery query, CancellationToken ct)
        {
            var result = await _service.GetAllAsync(query, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);
            return result is null ? NotFound(new { message = "Product not found" }) : Ok(result);
    }
    }
}
