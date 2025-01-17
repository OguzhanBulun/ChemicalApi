using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealerController : ControllerBase
    {
        private readonly IDealerService _dealerService;

        public DealerController(IDealerService dealerService)
        {
            _dealerService = dealerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dealers = await _dealerService.GetAllAsync();
            return Ok(dealers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dealer = await _dealerService.GetByIdAsync(id);
            return Ok(dealer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DealerDto dealerDto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var createdDealer = await _dealerService.CreateAsync(dealerDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdDealer.Id }, createdDealer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DealerDto dealerDto)
        {
            if (id != dealerDto.Id)
                return BadRequest();

            await _dealerService.UpdateAsync(dealerDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _dealerService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("taxnumber/{taxNumber}")]
        public async Task<IActionResult> GetByTaxNumber(string taxNumber)
        {
            var dealer = await _dealerService.GetByTaxNumberAsync(taxNumber);
            return Ok(dealer);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var dealers = await _dealerService.GetActiveAsync();
            return Ok(dealers);
        }
    }
} 