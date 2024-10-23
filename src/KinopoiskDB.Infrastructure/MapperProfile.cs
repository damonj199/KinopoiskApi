using AutoMapper;

using KinopoiskDB.Application.Dtos;
using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Infrastructure;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Movie, MovieDto>();
    }
}
