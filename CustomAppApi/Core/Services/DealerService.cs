using AutoMapper;
using CustomAppApi.Core.Repositories;
using CustomAppApi.Core.UnitOfWork;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomAppApi.Core.Services
{
    public class DealerService : IDealerService
    {
        private readonly IGenericRepository<Dealer> _dealerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<User> _userRepository;

        public DealerService(IGenericRepository<Dealer> dealerRepository, IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<User> userRepository)
        {
            _dealerRepository = dealerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<DealerDto>> GetAllAsync()
        {
            var dealers = await _dealerRepository.GetAll()
                .Include(d => d.User)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DealerDto>>(dealers);
        }

        public async Task<DealerDto> GetByIdAsync(int id)
        {
            var dealer = await _dealerRepository.GetAll()
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dealer == null)
                throw new KeyNotFoundException($"Dealer with ID {id} not found.");
                
            var dealerDto = _mapper.Map<DealerDto>(dealer);
            dealerDto.Id = dealer.Id;
            return dealerDto;
        }

        public async Task<DealerDto> CreateAsync(DealerDto dealerDto)
        {
            try 
            {
                var exists = await ExistsAsync(dealerDto.TaxNumber, dealerDto.Email);
                if (exists)
                    throw new InvalidOperationException($"Dealer with tax number {dealerDto.TaxNumber} or email {dealerDto.Email} already exists.");

                // User'ın var olduğunu kontrol et
                var user = await _userRepository.GetByIdAsync(dealerDto.UserId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {dealerDto.UserId} not found.");

                if (user.UserType != UserType.Dealer)
                    throw new InvalidOperationException("Selected user must be a dealer type user.");

                var dealer = _mapper.Map<Dealer>(dealerDto);
                await _dealerRepository.AddAsync(dealer);
                await _unitOfWork.CommitAsync();
                
                return await GetByIdAsync(dealer.Id);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error occurred while creating dealer.", ex);
            }
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
                .Include(d => d.User)
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
                .Include(d => d.User)
                .Where(d => d.IsActive)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DealerDto>>(dealers);
        }
    }
} 