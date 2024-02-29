using AutoMapper;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using MongoDB.Bson;

namespace NoSQLSkiServiceManager.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping von DTOs zu deiner AccountHolder-Entität
            CreateMap<AccountHolderCreateDto, AccountHolder>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.GenerateNewId().ToString()))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.InitialBalance));

            CreateMap<AccountBalanceUpdateDto, AccountHolder>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.NewBalance))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Mapping von deiner AccountHolder-Entität zu DTOs
            CreateMap<AccountHolder, AccountHolderResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<AccountHolder, LoginResponseDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore()) 
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
                .ForMember(dest => dest.FailedLoginAttempts, opt => opt.MapFrom(src => src.FailedLoginAttempts))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
        }
    }
}
