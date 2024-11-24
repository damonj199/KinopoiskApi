using KinopoiskDB.Dal.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace MoviesRepository.Tests.Intergation;

public class DbContextOptions
{
    public DbContextOptions<KinopoiskDbContext> CreateInMemoryDatabase()
    {
        var options = new DbContextOptionsBuilder<KinopoiskDbContext>()
            .UseNpgsql("Host=postgreSQL;Port=5432;Database=testDb;Username=postgres;password=12345")
            .Options;

        using var context = new KinopoiskDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return options;
    }
}