using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Common;

namespace CustomAppApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class DealerController : ControllerBase
    {
        private readonly IDealerService _dealerService;

        public DealerController(IDealerService dealerService)
        {
            _dealerService = dealerService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Personnel")]
        public async Task<ActionResult<ApiResponse<DealerDto>>> Create(DealerDto dealerDto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var createdDealer = await _dealerService.CreateAsync(dealerDto, userId);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = createdDealer.Id }, 
                ApiResponse<DealerDto>.SuccessResult(createdDealer, "Bayi başarıyla oluşturuldu"));
        }

        [HttpPost("{dealerId}/products")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> AddProducts(int dealerId, [FromBody] List<int> productIds)
        {
            await _dealerService.AddProductsToDealerAsync(dealerId, productIds);
            return Ok(ApiResponse<object>.SuccessResult(null, "Ürünler bayiye başarıyla eklendi"));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Personnel")]
        public async Task<ActionResult<ApiResponse<DealerDto>>> GetById(int id)
        {
            var dealer = await _dealerService.GetByIdAsync(id);
            return Ok(ApiResponse<DealerDto>.SuccessResult(dealer));
        }

        // ... diğer endpoint'ler ...
    }
} 