using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.Entities;

namespace BusinessLayer.BusinessModels
{
    public class CustomMapper
    {
        public static IMapper Configurate()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<City, CityDTO>().PreserveReferences();
                cfg.CreateMap<Country, CountryDTO>().PreserveReferences();
                cfg.CreateMap<Friend, FriendDTO>().PreserveReferences();
                cfg.CreateMap<Language, LanguageDTO>();
                cfg.CreateMap<Sex, SexDTO>();
                cfg.CreateMap<UserMessage, UserMessageDTO>().PreserveReferences();
                cfg.CreateMap<UserProfile, UserProfileDTO>().PreserveReferences();

                cfg.CreateMap<CityDTO, City>().PreserveReferences();
                cfg.CreateMap<CountryDTO, Country>().PreserveReferences();
                cfg.CreateMap<FriendDTO, Friend>().PreserveReferences();
                cfg.CreateMap<LanguageDTO, Language>();
                cfg.CreateMap<SexDTO, Sex>();
                cfg.CreateMap<UserMessageDTO, UserMessage>().PreserveReferences();
                cfg.CreateMap<UserProfileDTO, UserProfile>().PreserveReferences();

            });
            return config.CreateMapper();
        }
    }
}
