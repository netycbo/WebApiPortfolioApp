using AutoMapper;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data.Entinities;
using WebApiPortfolioApp.Data.Entinities.Identity;

namespace WebApiPortfolioApp.API.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<ApplicationUser, RegisteringDto>()
                .ForMember(x => x.Name, x => x.MapFrom(y => y.UserName))
                .ForMember(x => x.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(x => x.BeerAlert, opt => opt.MapFrom(src => src.IsSubscribedToNewsLetter));

            CreateMap<RegisteringRequest, RegisteringDto>()
                .ForMember(x => x.Name, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(x => x.BeerAlert, y => y.MapFrom(y => y.SubscribeToMailingList));

            CreateMap<RegisteringRequest, ApplicationUser>()
                .ForMember(x => x.UserName, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.Email, x => x.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, LoginDto>()
                .ForMember(x => x.Name, x => x.MapFrom(y => y.UserName));

            CreateMap<LoginRequest, LoginDto>();

            CreateMap<RawJsonDtoResponse, List<RawJsonDto>>()
                .ConvertUsing((src, dest, context) => context.Mapper.Map<List<RawJsonDto>>(src.Data));

            CreateMap<RawJsonDto, AddProductsToNewsLetterDto>()
                .ForMember(x => x.ProductName, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.Price, x => x.MapFrom(y => y.Current_Price))
                 .ForMember(dest => dest.Store, opt => opt.MapFrom(y => new StoreName { Name = y.Store.Name }));
            CreateMap<List<RawJsonDto>, List<AddProductsToNewsLetterDto>>()
             .ConvertUsing((src, dest, context) =>
             {
                 var mappedList = src.Select(item => context.Mapper.Map<AddProductsToNewsLetterDto>(item)).ToList();
                 return mappedList;
             });
            CreateMap<RawJsonDto, UpdatePriceProduktDto>()
                .ForMember(x => x.ProductName, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.Price, x => x.MapFrom(y => y.Current_Price))
                .ForMember(x => x.Store, x => x.MapFrom(y => new StoreName { Name = y.Store.Name }));
            CreateMap<List<RawJsonDto>, UpdatePriceProduktDto>()
                .ForMember(x => x.ProductName, x => x.MapFrom(y => y))
                .ForMember(x => x.Price, x => x.MapFrom(y => y))
                .ForMember(x => x.Store, x => x.MapFrom(y => y));

            CreateMap<TemporaryProductsDto, UpdatePriceProduktDto>()
                .ForMember(x => x.ProductName, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.Price, x => x.MapFrom(y => y.Price))
                .ForMember(x => x.Store, x => x.MapFrom(y => new StoreName { Name = y.Store.Name }));

            CreateMap<UpdatePriceProduktDto, TemporaryProductsDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Store, x => x.MapFrom(y => new StoreName { Name = y.Store.Name }));

        }
    }
}
