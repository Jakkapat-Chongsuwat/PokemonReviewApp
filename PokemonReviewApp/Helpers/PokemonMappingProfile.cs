using AutoMapper;
using PokemonReviewApp.Data.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Profiles
{
    public class PokemonMappingProfile : Profile
    {
        public PokemonMappingProfile()
        {
            CreateMap<Pokemon, PokemonDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Country, CountryDto>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<Owner, OwnerWithCountryDto>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OwnerFirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.OwnerLastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.OwnerGym, opt => opt.MapFrom(src => src.Gym))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country.Id))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name));
            CreateMap<Reviewer, ReviewerDto>()
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));
            CreateMap<Review, ReviewDto>(); // Ensure this doesn't map back to ReviewerDto
        }
    }
}
