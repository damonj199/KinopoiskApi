using AutoMapper;

using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Infrastructure;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Movie, MovieDto>();
        CreateMap<MovieDto, Movie>();

        CreateMap<Genre, GenreDto>()
            .ForMember(a => a.Genre, b => b.MapFrom(s => s.Value));
        CreateMap<GenreDto, Genre>()
            .ForMember(a => a.Value, b => b.MapFrom(s => s.Genre));

        CreateMap<Country, CountryDto>()
            .ForMember(a => a.Country, b => b.MapFrom(s => s.Value));
        CreateMap<CountryDto, Country>()
            .ForMember(a => a.Value, b => b.MapFrom(s => s.Country));
    }
}
