﻿using AutoMapper;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data.Entinities.Identity;

namespace WebApiPortfolioApp.API.Mappings
{
    public class Profiles : Profile
    {
        public Profiles() 
        {
            CreateMap<ApplicationUser, RegisteringDto>()
                    .ForMember(x => x.Name, x => x.MapFrom(y => y.UserName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.BeerAlert, opt => opt.MapFrom(src => src.IsSubscribedToLowBeerPriceAletr));


            CreateMap<RegisteringRequest, RegisteringDto>()
                   .ForMember(x => x.Name, x => x.MapFrom(y => y.Name))
                   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                   .ForMember(x => x.BeerAlert, y => y.MapFrom(y => y.SubscribeToMailingList));
                   

            CreateMap<RegisteringRequest, ApplicationUser>()
                   .ForMember(x => x.UserName, x => x.MapFrom(y => y.Name))
                   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<ApplicationUser, LoginDto>()
                .ForMember(x => x.Name, x => x.MapFrom(y => y.UserName));
            CreateMap<LoginRequest, LoginDto>();
            CreateMap<RawJsonDtoResponse, List<RawJsonDto>>();
            CreateMap<string, ProductNamesDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src));



        }
    }
}