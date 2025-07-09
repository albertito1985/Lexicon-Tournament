using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using AutoMapper;
using Tournament.Core.Request;

namespace Tournament.Data.Data
{
    public class TournamentMappings : Profile
    {
        public TournamentMappings()
        {
            CreateMap<TournamentDetails, TournamentDetailsDTO>()
                .ForMember(
                    dest=> dest.EndDate,
                    opt=>opt.MapFrom(src => src.StartDate.AddMonths(3)))
                .ReverseMap();
            CreateMap<Game, GameDTO>().ReverseMap();
            CreateMap<TournamentDetails, TournamentUpdateDTO>().ReverseMap();
            CreateMap<Game, GameUpdateDTO>().ReverseMap();
        }
    }
}
