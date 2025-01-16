using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Core.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleDto>> GetAllAsync();
        Task<SaleDto> GetByIdAsync(int id);
        Task<SaleDto> CreateAsync(SaleDto saleDto);
        Task UpdateAsync(SaleDto saleDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<SaleDto>> GetByDealerAsync(int dealerId);
        Task<IEnumerable<SaleDto>> GetByProductAsync(int productId);
        Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalSaleAmountAsync(int dealerId);
        Task<decimal> GetTotalSaleAmountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<SaleDto> CreateFromOfferAsync(int offerId);
    }
} 