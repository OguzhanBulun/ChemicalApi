using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Core.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductDto productDto);
        Task UpdateAsync(ProductDto productDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductDto>> GetByNameAsync(string name);
        Task<IEnumerable<ProductDto>> GetActiveAsync();
        Task<IEnumerable<ProductDto>> GetByUnitAsync(string unit);
        Task<bool> ExistsAsync(string name);
    }
} 