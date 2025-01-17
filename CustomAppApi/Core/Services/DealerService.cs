using AutoMapper;
using CustomAppApi.Core.Repositories;
using CustomAppApi.Core.UnitOfWork;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using CustomAppApi.Data;

namespace CustomAppApi.Core.Services
{
    public class DealerService : IDealerService
    {
        private readonly IGenericRepository<Dealer> _dealerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ApplicationDbContext _context;

        public DealerService(IGenericRepository<Dealer> dealerRepository, IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<User> userRepository, ApplicationDbContext context)
        {
            _dealerRepository = dealerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<DealerDto>> GetAllAsync()
        {
            var dealers = await _dealerRepository.GetAll()
                .Include(d => d.AssignedUser)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DealerDto>>(dealers);
        }

        public async Task<DealerDto> GetByIdAsync(int id)
        {
            var dealer = await _dealerRepository.GetAll()
                .Include(d => d.AssignedUser)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dealer == null)
                throw new KeyNotFoundException($"Dealer with ID {id} not found.");
                
            var dealerDto = _mapper.Map<DealerDto>(dealer);
            dealerDto.Id = dealer.Id;
            return dealerDto;
        }

        public async Task<DealerDto> CreateAsync(DealerDto dealerDto, int createdByUserId)
        {
            if (await ExistsAsync(dealerDto.TaxNumber, dealerDto.Email))
                throw new InvalidOperationException("Dealer with same tax number or email already exists.");

            var dealer = _mapper.Map<Dealer>(dealerDto);
            dealer.CreatedByUserId = createdByUserId;
            
            await _dealerRepository.AddAsync(dealer);
            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<DealerDto>(dealer);
        }

        public async Task UpdateAsync(DealerDto dealerDto)
        {
            if (!dealerDto.Id.HasValue)
                throw new ArgumentException("Dealer ID cannot be null");

            var existingDealer = await _dealerRepository.GetByIdAsync(dealerDto.Id.Value);
            if (existingDealer == null)
                throw new KeyNotFoundException($"Dealer with ID {dealerDto.Id} not found.");

            if (existingDealer.TaxNumber != dealerDto.TaxNumber || existingDealer.Email != dealerDto.Email)
            {
                var exists = await ExistsAsync(dealerDto.TaxNumber, dealerDto.Email);
                if (exists)
                    throw new InvalidOperationException("Tax number or email already exists.");
            }

            _mapper.Map(dealerDto, existingDealer);
            _dealerRepository.Update(existingDealer);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id);
            if (dealer == null)
                throw new KeyNotFoundException($"Dealer with ID {id} not found.");

            dealer.IsActive = false; 
            _dealerRepository.Update(dealer);
            await _unitOfWork.CommitAsync();
        }

        public async Task<DealerDto> GetByTaxNumberAsync(string taxNumber)
        {
            var dealer = await _dealerRepository.GetAll()
                .Include(d => d.AssignedUser)
                .FirstOrDefaultAsync(d => d.TaxNumber == taxNumber);

            if (dealer == null)
                throw new KeyNotFoundException($"Dealer with tax number {taxNumber} not found.");
                
            return _mapper.Map<DealerDto>(dealer);
        }

        public async Task<bool> ExistsAsync(string taxNumber, string email)
        {
            return await _dealerRepository.AnyAsync(d => 
                d.TaxNumber == taxNumber || d.Email == email);
        }

        public async Task<IEnumerable<DealerDto>> GetActiveAsync()
        {
            var dealers = await _dealerRepository.GetAll()
                .Include(d => d.AssignedUser)
                .Where(d => d.IsActive)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DealerDto>>(dealers);
        }

        public async Task AddProductsToDealerAsync(int dealerId, List<int> productIds)
        {
            var dealer = await _dealerRepository.GetByIdAsync(dealerId);
            if (dealer == null)
                throw new KeyNotFoundException($"Dealer with ID {dealerId} not found.");

            var dealerProducts = productIds.Select(productId => new DealerProduct
            {
                DealerId = dealerId,
                ProductId = productId
            });

            await _context.DealerProducts.AddRangeAsync(dealerProducts);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<DealerDto>> GetAllWithProductsAsync()
        {
            var dealers = await _dealerRepository.GetAll()
                .Include(d => d.DealerProducts)
                .ThenInclude(dp => dp.Product)
                .Where(d => d.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DealerDto>>(dealers);
        }

        public async Task AssignUserToDealerAsync(int dealerId, int userId)
        {
            var dealer = await _dealerRepository.GetByIdAsync(dealerId);
            if (dealer == null)
                throw new KeyNotFoundException($"Dealer with ID {dealerId} not found.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            dealer.UserId = userId;
            _dealerRepository.Update(dealer);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<DealerDto>> GetDealersByUserIdAsync(int userId)
        {
            var dealers = await _dealerRepository.GetAll()
                .Include(d => d.AssignedUser)
                .Include(d => d.DealerProducts)
                .ThenInclude(dp => dp.Product)
                .Where(d => d.UserId == userId && d.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DealerDto>>(dealers);
        }
    }
} 