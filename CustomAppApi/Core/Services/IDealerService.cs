using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Core.Services
{
    public interface IDealerService
    {
        Task<IEnumerable<DealerDto>> GetAllAsync();
        Task<DealerDto> GetByIdAsync(int id);
        Task<DealerDto> CreateAsync(DealerDto dealerDto);
        Task UpdateAsync(DealerDto dealerDto);
        Task DeleteAsync(int id);
        Task<DealerDto> GetByTaxNumberAsync(string taxNumber);
        Task<bool> ExistsAsync(string taxNumber, string email);
        Task<IEnumerable<DealerDto>> GetActiveAsync();
    }
} 