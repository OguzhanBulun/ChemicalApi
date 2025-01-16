using AutoMapper;
using CustomAppApi.Core.Repositories;
using CustomAppApi.Core.UnitOfWork;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomAppApi.Core.Services
{
    public class SaleService : ISaleService
    {
        private readonly IGenericRepository<Sale> _saleRepository;
        private readonly IGenericRepository<Offer> _offerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SaleService(
            IGenericRepository<Sale> saleRepository,
            IGenericRepository<Offer> offerRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _offerRepository = offerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SaleDto>> GetAllAsync()
        {
            var sales = await _saleRepository.GetAll()
                .Include(s => s.Dealer)
                .Include(s => s.Product)
                .ToListAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<SaleDto> GetByIdAsync(int id)
        {
            var sale = await _saleRepository.GetAll()
                .Include(s => s.Dealer)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {id} not found.");
                
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<SaleDto> CreateAsync(SaleDto saleDto)
        {
            var sale = _mapper.Map<Sale>(saleDto);
            sale.SaleDate = DateTime.UtcNow;
            sale.TotalPrice = sale.UnitPrice * sale.Quantity;
            
            await _saleRepository.AddAsync(sale);
            await _unitOfWork.CommitAsync();
            
            return await GetByIdAsync(sale.Id);
        }

        public async Task UpdateAsync(SaleDto saleDto)
        {
            var existingSale = await _saleRepository.GetByIdAsync(saleDto.Id);
            if (existingSale == null)
                throw new KeyNotFoundException($"Sale with ID {saleDto.Id} not found.");

            _mapper.Map(saleDto, existingSale);
            existingSale.TotalPrice = existingSale.UnitPrice * existingSale.Quantity;
            
            _saleRepository.Update(existingSale);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {id} not found.");

            sale.IsActive = false;
            _saleRepository.Update(sale);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<SaleDto>> GetByDealerAsync(int dealerId)
        {
            var sales = await _saleRepository.GetAll()
                .Include(s => s.Dealer)
                .Include(s => s.Product)
                .Where(s => s.DealerId == dealerId && s.IsActive)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByProductAsync(int productId)
        {
            var sales = await _saleRepository.GetAll()
                .Include(s => s.Dealer)
                .Include(s => s.Product)
                .Where(s => s.ProductId == productId && s.IsActive)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _saleRepository.GetAll()
                .Include(s => s.Dealer)
                .Include(s => s.Product)
                .Where(s => s.SaleDate >= startDate && 
                           s.SaleDate <= endDate && 
                           s.IsActive)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<decimal> GetTotalSaleAmountAsync(int dealerId)
        {
            return await _saleRepository.GetAll()
                .Where(s => s.DealerId == dealerId && s.IsActive)
                .SumAsync(s => s.TotalPrice);
        }

        public async Task<decimal> GetTotalSaleAmountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _saleRepository.GetAll()
                .Where(s => s.SaleDate >= startDate && 
                           s.SaleDate <= endDate && 
                           s.IsActive)
                .SumAsync(s => s.TotalPrice);
        }

        public async Task<SaleDto> CreateFromOfferAsync(int offerId)
        {
            var offer = await _offerRepository.GetAll()
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (offer == null)
                throw new KeyNotFoundException($"Offer with ID {offerId} not found.");

            if (offer.Status != OfferStatus.Accepted)
                throw new InvalidOperationException("Only accepted offers can be converted to sales.");

            var sale = new Sale
            {
                DealerId = offer.DealerId,
                ProductId = offer.ProductId,
                Quantity = offer.Quantity,
                UnitPrice = offer.Price,
                TotalPrice = offer.Price * offer.Quantity,
                SaleDate = DateTime.UtcNow,
                IsActive = true
            };

            await _saleRepository.AddAsync(sale);
            await _unitOfWork.CommitAsync();

            return await GetByIdAsync(sale.Id);
        }
    }
} 