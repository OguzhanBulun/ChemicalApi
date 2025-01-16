using AutoMapper;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Dealer, DealerDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Offer, OfferDto>().ReverseMap();
            CreateMap<Sale, SaleDto>().ReverseMap();
        }
    }
} 