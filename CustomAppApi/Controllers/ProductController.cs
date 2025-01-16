using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            var createdProduct = await _productService.CreateAsync(productDto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest();

            await _productService.UpdateAsync(productDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var products = await _productService.GetByNameAsync(name);
            return Ok(products);
        }

        [HttpGet("unit/{unit}")]
        public async Task<IActionResult> GetByUnit(string unit)
        {
            var products = await _productService.GetByUnitAsync(unit);
            return Ok(products);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var products = await _productService.GetActiveAsync();
            return Ok(products);
        }
    }
} 