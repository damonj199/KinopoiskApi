using KinopoiskDB.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace KinopoiskDB.Dal.PostgreSQL;

public class KinopoiskDbContext : DbContext
{
    public KinopoiskDbContext(DbContextOptions<KinopoiskDbContext> options) : base(options) { }

    public DbSet<Countries> Countries { get; set; }
    public DbSet<Genres> Genres { get; set; }
    public DbSet<Movies> Movies { get; set; }
}
