using AutoMapper;
using CustomAppApi.Core.Repositories;
using CustomAppApi.Core.UnitOfWork;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomAppApi.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IGenericRepository<Product> productRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAll().ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");
                
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(ProductDto productDto)
        {
            var exists = await ExistsAsync(productDto.Name);
            if (exists)
                throw new InvalidOperationException($"Product with name {productDto.Name} already exists.");

            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product);
            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(productDto.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");

            if (existingProduct.Name != productDto.Name)
            {
                var exists = await ExistsAsync(productDto.Name);
                if (exists)
                    throw new InvalidOperationException($"Product with name {productDto.Name} already exists.");
            }

            _mapper.Map(productDto, existingProduct);
            _productRepository.Update(existingProduct);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            product.IsActive = false;
            _productRepository.Update(product);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetByNameAsync(string name)
        {
            var products = await _productRepository.GetAll()
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetActiveAsync()
        {
            var products = await _productRepository.GetAll()
                .Where(p => p.IsActive)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetByUnitAsync(string unit)
        {
            var products = await _productRepository.GetAll()
                .Where(p => p.Unit == unit && p.IsActive)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _productRepository.AnyAsync(p => p.Name == name);
        }

        public async Task<ProductDto> GetByIdWithDealersAsync(int id)
        {
            var product = await _productRepository.GetAll()
                .Include(p => p.DealerProducts)
                .ThenInclude(dp => dp.Dealer)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            return _mapper.Map<ProductDto>(product);
        }
    }
} 