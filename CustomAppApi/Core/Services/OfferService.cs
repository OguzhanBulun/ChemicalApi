using AutoMapper;
using CustomAppApi.Core.Repositories;
using CustomAppApi.Core.UnitOfWork;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomAppApi.Core.Services
{
    public class OfferService : IOfferService
    {
        private readonly IGenericRepository<Offer> _offerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OfferService(IGenericRepository<Offer> offerRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _offerRepository = offerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OfferDto>> GetAllAsync()
        {
            var offers = await _offerRepository.GetAll()
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OfferDto>>(offers);
        }

        public async Task<OfferDto> GetByIdAsync(int id)
        {
            var offer = await _offerRepository.GetAll()
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offer == null)
                throw new KeyNotFoundException($"Offer with ID {id} not found.");
                
            return _mapper.Map<OfferDto>(offer);
        }

        public async Task<OfferDto> CreateAsync(OfferDto offerDto)
        {
            var offer = _mapper.Map<Offer>(offerDto);
            offer.OfferDate = DateTime.UtcNow;
            offer.Status = OfferStatus.Pending;
            
            await _offerRepository.AddAsync(offer);
            await _unitOfWork.CommitAsync();
            
            return await GetByIdAsync(offer.Id); 
        }

        public async Task UpdateAsync(OfferDto offerDto)
        {
            var existingOffer = await _offerRepository.GetByIdAsync(offerDto.Id);
            if (existingOffer == null)
                throw new KeyNotFoundException($"Offer with ID {offerDto.Id} not found.");

            if (existingOffer.Status != offerDto.Status && offerDto.Status == OfferStatus.Accepted)
            {
                offerDto.OfferDate = DateTime.UtcNow;
            }

            _mapper.Map(offerDto, existingOffer);
            _offerRepository.Update(existingOffer);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var offer = await _offerRepository.GetByIdAsync(id);
            if (offer == null)
                throw new KeyNotFoundException($"Offer with ID {id} not found.");

            offer.IsActive = false; // Soft delete
            _offerRepository.Update(offer);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<OfferDto>> GetByDealerAsync(int dealerId)
        {
            var offers = await _offerRepository.GetAll()
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .Where(o => o.DealerId == dealerId && o.IsActive)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OfferDto>>(offers);
        }

        public async Task<IEnumerable<OfferDto>> GetByProductAsync(int productId)
        {
            var offers = await _offerRepository.GetAll()
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .Where(o => o.ProductId == productId && o.IsActive)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OfferDto>>(offers);
        }

        public async Task<IEnumerable<OfferDto>> GetByStatusAsync(OfferStatus status)
        {
            var offers = await _offerRepository.GetAll()
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .Where(o => o.Status == status && o.IsActive)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OfferDto>>(offers);
        }

        public async Task<OfferDto> UpdateStatusAsync(int id, OfferStatus newStatus)
        {
            var offer = await _offerRepository.GetByIdAsync(id);
            if (offer == null)
                throw new KeyNotFoundException($"Offer with ID {id} not found.");

            offer.Status = newStatus;
            if (newStatus == OfferStatus.Accepted)
            {
                offer.OfferDate = DateTime.UtcNow;
            }

            _offerRepository.Update(offer);
            await _unitOfWork.CommitAsync();

            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<OfferDto>> GetActiveDealerOffersAsync(int dealerId)
        {
            var offers = await _offerRepository.GetAll()
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .Where(o => o.DealerId == dealerId && 
                           o.IsActive && 
                           o.Status == OfferStatus.Pending)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OfferDto>>(offers);
        }

        public async Task<decimal> GetTotalOfferAmountAsync(int dealerId)
        {
            return await _offerRepository.GetAll()
                .Where(o => o.DealerId == dealerId && 
                           o.IsActive && 
                           o.Status == OfferStatus.Pending)
                .SumAsync(o => o.Price * o.Quantity);
        }
    }
} 