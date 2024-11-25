using KinopoiskDB.Core.Models;
using KinopoiskDB.Dal.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MoviesRepositoryTests.Intergation;

public class DataBaseFixture : IDisposable
{
    public KinopoiskDbContext Context { get; private set; }
    private readonly IDbContextTransaction _transaction;

    public DataBaseFixture()
    {
        var options = new DbContextOptionsBuilder<KinopoiskDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=tests;Username=postgres;password=1234")
            .Options;

        Context = new KinopoiskDbContext(options);

        Context.Database.OpenConnection();

        SeedTestData();
        _transaction = Context.Database.BeginTransaction();
    }

    public void SeedTestData()
    {
        if(!Context.Movies.Any()) 
        {
            Context.Movies.AddRange(
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 1,
                Year = 2012,
                NameRu = "Фильм 1",
                NameEn = "Movie 1",
                PremiereRu = new DateOnly(2014, 10, 4),
                Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Испания" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 2,
                Year = 2015,
                NameRu = "Фильм 2",
                NameEn = "Movie 2",
                PremiereRu = new DateOnly(2024, 11, 7),
                Genres = new List<Genre> { new Genre { Value = "Боевик" } },
                Countries = new List<Country> { new Country { Value = "Бельгия" }, new Country { Value = "Италия" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 3,
                Year = 2011,
                NameRu = "Фильм 3",
                NameEn = "Movie 3",
                PremiereRu = new DateOnly(2014, 10, 4),
                Genres = new List<Genre> { new Genre { Value = "Комедия" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "США" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 4,
                Year = 2014,
                NameRu = "Фильм 4",
                NameEn = "Movie 4",
                PremiereRu = new DateOnly(2015, 1, 14),
                Genres = new List<Genre> { new Genre { Value = "Драмма" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Россия" }, new Country { Value = "Великобритания" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 5,
                Year = 2012,
                NameRu = "Фильм 5",
                NameEn = "Movie 5",
                PremiereRu = new DateOnly(2017, 3, 24),
                Genres = new List<Genre> { new Genre { Value = "Фэнтези" }, new Genre { Value = "Аниме" } },
                Countries = new List<Country> { new Country { Value = "Китай" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 6,
                Year = 2020,
                NameRu = "Фильм 6",
                NameEn = "Movie 6",
                PremiereRu = new DateOnly(2014, 10, 14),
                Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "США" }, new Country { Value = "Турция" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 7,
                Year = 2021,
                NameRu = "Фильм 7",
                NameEn = "Movie 7",
                PremiereRu = new DateOnly(2019, 11, 4),
                Genres = new List<Genre> { new Genre { Value = "Исторический" }, new Genre { Value = "Драмма" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Россия" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 8,
                Year = 2022,
                NameRu = "Фильм 8",
                NameEn = "Movie 8",
                PremiereRu = new DateOnly(2014, 12, 5),
                Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Иран" } }
            },
            new Movie
            {
                Id = Guid.NewGuid(),
                KinopoiskId = 9,
                Year = 2022,
                NameRu = "Фильм 9",
                NameEn = "Movie 9",
                PremiereRu = new DateOnly(2015, 11, 4),
                Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Великобритания" }, new Country { Value = "США" } }
            });
        };
        Context.SaveChanges();
    }

    public void Dispose()
    {
        _transaction.Rollback();
        _transaction.Dispose();
        Context.Database.CloseConnection();
        Context.Dispose();
    }
}
