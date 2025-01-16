using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _saleService.GetAllAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleDto saleDto)
        {
            var createdSale = await _saleService.CreateAsync(saleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSale.Id }, createdSale);
        }

        [HttpPost("from-offer/{offerId}")]
        public async Task<IActionResult> CreateFromOffer(int offerId)
        {
            var createdSale = await _saleService.CreateFromOfferAsync(offerId);
            return CreatedAtAction(nameof(GetById), new { id = createdSale.Id }, createdSale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SaleDto saleDto)
        {
            if (id != saleDto.Id)
                return BadRequest();

            await _saleService.UpdateAsync(saleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _saleService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> GetByDealer(int dealerId)
        {
            var sales = await _saleService.GetByDealerAsync(dealerId);
            return Ok(sales);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var sales = await _saleService.GetByProductAsync(productId);
            return Ok(sales);
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetByDateRangeAsync(startDate, endDate);
            return Ok(sales);
        }

        [HttpGet("dealer/{dealerId}/total")]
        public async Task<IActionResult> GetTotalSaleAmount(int dealerId)
        {
            var total = await _saleService.GetTotalSaleAmountAsync(dealerId);
            return Ok(total);
        }

        [HttpGet("total/date-range")]
        public async Task<IActionResult> GetTotalSaleAmountByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var total = await _saleService.GetTotalSaleAmountByDateRangeAsync(startDate, endDate);
            return Ok(total);
        }
    }
} 