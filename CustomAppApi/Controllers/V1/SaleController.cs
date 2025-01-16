using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Common;

namespace CustomAppApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [EnableRateLimiting("SaleLimit")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [EnableRateLimiting("GetLimit")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SaleDto>>>> GetAll()
        {
            var sales = await _saleService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<SaleDto>>.SuccessResult(sales));
        }

        [EnableRateLimiting("GetLimit")]
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Dealer")]
        public async Task<ActionResult<ApiResponse<SaleDto>>> GetById(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            
            if (User.IsInRole("Dealer"))
            {
                var userDealerId = User.FindFirst("DealerId")?.Value;
                if (userDealerId != sale.DealerId.ToString())
                    return Forbid();
            }

            return Ok(ApiResponse<SaleDto>.SuccessResult(sale));
        }

        [EnableRateLimiting("PostLimit")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<SaleDto>>> Create(SaleDto saleDto)
        {
            var createdSale = await _saleService.CreateAsync(saleDto);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = createdSale.Id }, 
                ApiResponse<SaleDto>.SuccessResult(createdSale, "Satış başarıyla oluşturuldu"));
        }

        [EnableRateLimiting("PostLimit")]
        [HttpPost("from-offer/{offerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<SaleDto>>> CreateFromOffer(int offerId)
        {
            var createdSale = await _saleService.CreateFromOfferAsync(offerId);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = createdSale.Id }, 
                ApiResponse<SaleDto>.SuccessResult(createdSale, "Teklif başarıyla satışa dönüştürüldü"));
        }

        [EnableRateLimiting("GetLimit")]
        [HttpGet("dealer/{dealerId}")]
        [Authorize(Roles = "Admin,Dealer")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SaleDto>>>> GetByDealer(int dealerId)
        {
            if (User.IsInRole("Dealer"))
            {
                var userDealerId = User.FindFirst("DealerId")?.Value;
                if (userDealerId != dealerId.ToString())
                    return Forbid();
            }

            var sales = await _saleService.GetByDealerAsync(dealerId);
            return Ok(ApiResponse<IEnumerable<SaleDto>>.SuccessResult(sales));
        }

        [EnableRateLimiting("GetLimit")]
        [HttpGet("date-range")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SaleDto>>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetByDateRangeAsync(startDate, endDate);
            return Ok(ApiResponse<IEnumerable<SaleDto>>.SuccessResult(sales));
        }

        [EnableRateLimiting("GetLimit")]
        [HttpGet("dealer/{dealerId}/total")]
        [Authorize(Roles = "Admin,Dealer")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetTotalSaleAmount(int dealerId)
        {
            if (User.IsInRole("Dealer"))
            {
                var userDealerId = User.FindFirst("DealerId")?.Value;
                if (userDealerId != dealerId.ToString())
                    return Forbid();
            }

            var total = await _saleService.GetTotalSaleAmountAsync(dealerId);
            return Ok(ApiResponse<decimal>.SuccessResult(total));
        }
    }
} 