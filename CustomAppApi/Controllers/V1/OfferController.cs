using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomAppApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [EnableRateLimiting("OfferLimit")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [EnableRateLimiting("GetLimit")]
        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetAll()
        {
            var offers = await _offerService.GetAllAsync();
            return Ok(offers);
        }

        [HttpGet("dealer/{dealerId}")]
        [Authorize(Roles = "Admin,Dealer")] 
        public async Task<IActionResult> GetByDealer(int dealerId)
        {
            
            if (User.IsInRole("Dealer"))
            {
                var userDealerId = User.FindFirst("DealerId")?.Value;
                if (userDealerId != dealerId.ToString())
                    return Forbid();
            }

            var offers = await _offerService.GetByDealerAsync(dealerId);
            return Ok(offers);
        }

        [EnableRateLimiting("PostLimit")]
        [HttpPost]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> Create(OfferDto offerDto)
        {
            var userDealerId = User.FindFirst("DealerId")?.Value;
            if (userDealerId != offerDto.DealerId.ToString())
                return Forbid();

            var createdOffer = await _offerService.CreateAsync(offerDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOffer.Id }, createdOffer);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var offer = await _offerService.GetByIdAsync(id);
            return Ok(offer);
        }
    }
} 