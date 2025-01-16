using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Core.Services
{
    public interface IOfferService
    {
        Task<IEnumerable<OfferDto>> GetAllAsync();
        Task<OfferDto> GetByIdAsync(int id);
        Task<OfferDto> CreateAsync(OfferDto offerDto);
        Task UpdateAsync(OfferDto offerDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<OfferDto>> GetByDealerAsync(int dealerId);
        Task<IEnumerable<OfferDto>> GetByProductAsync(int productId);
        Task<IEnumerable<OfferDto>> GetByStatusAsync(OfferStatus status);
        Task<OfferDto> UpdateStatusAsync(int id, OfferStatus newStatus);
        Task<IEnumerable<OfferDto>> GetActiveDealerOffersAsync(int dealerId);
        Task<decimal> GetTotalOfferAmountAsync(int dealerId);
    }
} 