using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var offers = await _offerService.GetAllAsync();
            return Ok(offers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var offer = await _offerService.GetByIdAsync(id);
            return Ok(offer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OfferDto offerDto)
        {
            var createdOffer = await _offerService.CreateAsync(offerDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOffer.Id }, createdOffer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OfferDto offerDto)
        {
            if (id != offerDto.Id)
                return BadRequest();

            await _offerService.UpdateAsync(offerDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _offerService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> GetByDealer(int dealerId)
        {
            var offers = await _offerService.GetByDealerAsync(dealerId);
            return Ok(offers);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var offers = await _offerService.GetByProductAsync(productId);
            return Ok(offers);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(OfferStatus status)
        {
            var offers = await _offerService.GetByStatusAsync(status);
            return Ok(offers);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OfferStatus newStatus)
        {
            var updatedOffer = await _offerService.UpdateStatusAsync(id, newStatus);
            return Ok(updatedOffer);
        }

        [HttpGet("dealer/{dealerId}/active")]
        public async Task<IActionResult> GetActiveDealerOffers(int dealerId)
        {
            var offers = await _offerService.GetActiveDealerOffersAsync(dealerId);
            return Ok(offers);
        }

        [HttpGet("dealer/{dealerId}/total")]
        public async Task<IActionResult> GetTotalOfferAmount(int dealerId)
        {
            var total = await _offerService.GetTotalOfferAmountAsync(dealerId);
            return Ok(total);
        }
    }
} 