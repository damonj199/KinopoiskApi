using KinopoiskDB.Core.Dtos;

using Microsoft.EntityFrameworkCore;

namespace KinopoiskDB.Dal;

public class KinopoiskDbContext : DbContext
{
    public KinopoiskDbContext(DbContextOptions<KinopoiskDbContext> options) : base(options) { }

    public DbSet<CountriesDto> Countries { get; set; }
    public DbSet<GenresDto> Genres { get; set; }
    public DbSet<MoviesDto> Movies { get; set; }
}
