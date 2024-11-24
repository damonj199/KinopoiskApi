using FluentAssertions;
using KinopoiskDB.Core.Models;
using KinopoiskDB.Dal.PostgreSQL;
using KinopoiskDB.Dal.PostgreSQL.Repository;
using Microsoft.EntityFrameworkCore;

namespace MoviesRepositoryTests;

public class MoviesRepositoryTests
{
    private readonly DbContextOptions<KinopoiskDbContext> _dboptions;

    public MoviesRepositoryTests()
    {
        _dboptions = new DbContextOptionsBuilder<KinopoiskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact(DisplayName = "При наличии в списке уникального фильма, добавляем его в базу")]
    public async Task AddMoviesAsync_UniqueMoviesList_ShouldAddIfThereIsNoMovie()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 111, NameRu = "фильм 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 222, NameRu = "фильм 2", PremiereRu = new DateOnly(2024, 11, 15) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 333, NameRu = "фильм 3", PremiereRu = new DateOnly(2024, 12, 1) }
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var filmNew = new List<Movie> { new Movie { Id = Guid.NewGuid(), KinopoiskId = 123, NameRu = "какой то новый фильм", PremiereRu = new DateOnly(2024, 12, 31) } };

        //Actions
        var result = await repo.AddMoviesAsync(filmNew, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Should().Contain(m => m.KinopoiskId == 123);
    }

    [Fact(DisplayName = "При наличии фильмов обновляем данные о них")]
    public async Task AddMoviesAsync_ShouldUpdateIfThereIsMovieAndAdd()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 111, NameRu = "фильм 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 222, NameRu = "фильм 2", PremiereRu = new DateOnly(2024, 11, 15) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 333, NameRu = "фильм 3", PremiereRu = new DateOnly(2024, 12, 1) }
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var filmNew = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 222, NameRu = "какая то новинка", PremiereRu = new DateOnly(2024, 12, 31) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 111, NameRu = "Cалый лучший фильм 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 123, NameRu = "Самый новый", PremiereRu = new DateOnly(2022, 1, 1) },
        };

        //Actions
        var result = await repo.AddMoviesAsync(filmNew, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
        result.Should().Contain(m => m.PremiereRu == new DateOnly(2024, 12, 31));
        result.Should().Contain(m => m.NameRu == "Самый новый");
        result.Should().Contain(m => m.KinopoiskId == 123);

    }

    [Fact]
    public async Task GetPremieresForMonthAsync_ShouldReturnMoviesWithinSpecifiedDateRange()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "фильм 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), NameRu = "фильм 2", PremiereRu = new DateOnly(2024, 11, 15) },
            new Movie { Id = Guid.NewGuid(), NameRu = "фильм 3", PremiereRu = new DateOnly(2024, 12, 1) }
        };

        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repository = new MoviesRepository(context);
        var start = new DateOnly(2024, 11, 1);
        var end = new DateOnly(2024, 11, 30);

        //Actions
        var result = await repository.GetPremieresForMonthAsync(start, end, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().Contain(m => m.NameRu == "фильм 1");
        result.Should().Contain(m => m.NameRu == "фильм 2");
    }

    [Fact]
    public async Task SearchMoviesByNameAsync_ShouldReturnMoviesMatchingTitle()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie{Id = Guid.NewGuid(), NameRu = "фильм первый", NameEn = "Films One"},
            new Movie{Id = Guid.NewGuid(), NameRu = "фильм второй", NameEn = "Films Two"},
            new Movie{Id = Guid.NewGuid(), NameRu = "другой", NameEn = "Unrelated"},
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        //Actions
        var result = await repo.SearchMoviesByNameAsync("фильм", CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().Contain(m => m.NameEn == "Films One");
        result.Should().Contain(m => m.NameEn == "Films Two");
    }

    [Fact]
    public async Task GetAllMoviesAsync_ShouldReturnAllMovies()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var allMovies = new List<Movie>
        {
            new Movie {Id = Guid.NewGuid(), Year = 2012, NameRu = "Фильм 1", PremiereRu = new DateOnly(2014, 10, 4)},
            new Movie {Id = Guid.NewGuid(), Year = 2015, NameRu = "Фильм 2", PremiereRu = new DateOnly(2024, 11, 7)},
            new Movie {Id = Guid.NewGuid(), Year = 2011, NameRu = "Фильм 3", PremiereRu = new DateOnly(2014, 10, 4)},
            new Movie {Id = Guid.NewGuid(), Year = 2014, NameRu = "Фильм 4", PremiereRu = new DateOnly(2015, 1, 14)},
            new Movie {Id = Guid.NewGuid(), Year = 2012, NameRu = "Фильм 5", PremiereRu = new DateOnly(2017, 3, 24)},
            new Movie {Id = Guid.NewGuid(), Year = 2020, NameRu = "Фильм 6", PremiereRu = new DateOnly(2014, 10, 14)},
            new Movie {Id = Guid.NewGuid(), Year = 2021, NameRu = "Фильм 7", PremiereRu = new DateOnly(2019, 11, 4)},
            new Movie {Id = Guid.NewGuid(), Year = 2022, NameRu = "Фильм 8", PremiereRu = new DateOnly(2014, 12, 5)},
            new Movie {Id = Guid.NewGuid(), Year = 2022, NameRu = "Фильм 9", PremiereRu = new DateOnly(2015, 11, 4)}
        };
        context.Movies.AddRange(allMovies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        //Actions
        var result = await repo.GetAllMoviesAsync(2, 2, KinopoiskDB.Core.Enum.SortableFields.Year, KinopoiskDB.Core.Enum.SortOrder.ASC, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().Contain(y => y.Year == 2014);
        result.Should().Contain(y => y.Year == 2012);
    }

    [Fact]
    public async Task GetAllMoviesAsync_ShouldThrowException_WhenPageOrPageSizeIsInvalid()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var repo = new MoviesRepository(context);

        //Action Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repo.GetAllMoviesAsync(0, 5, KinopoiskDB.Core.Enum.SortableFields.Year, KinopoiskDB.Core.Enum.SortOrder.ASC, CancellationToken.None));
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnAllMovies_WhenNoFiltersProvided()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 1", Genres = new List<Genre> { new Genre { Value = "Боевик" } },
                Countries = new List<Country> { new Country { Value = "США" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 2", Genres = new List<Genre> { new Genre { Value = "Комедия" } },
                Countries = new List<Country> { new Country { Value = "Франция" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 3", Genres = new List<Genre> { new Genre { Value = "Ужасы" }, new Genre { Value = "Комедия" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Иран" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        //Action
        var result = await repo.GetMoviesByFilterAsync(null, "", CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnMoviesByGenres_WhenGenresFilterProvided()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 1", Genres = new List<Genre> { new Genre { Value = "Боевик" } },
                Countries = new List<Country> { new Country { Value = "США" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 2", Genres = new List<Genre> { new Genre { Value = "Комедия" } },
                Countries = new List<Country> { new Country { Value = "Франция" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 3", Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Иран" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync("Боевик", "", CancellationToken.None);

        Assert.NotNull(result);
        result.Count.Should().Be(2);
        result.Should().Contain(m => m.NameRu == "Фильм 1");
        result.Should().Contain(m => m.NameRu == "Фильм 3");
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnMoviesByCountries_WhenCountriesFilterProvided()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 1", Genres = new List<Genre> { new Genre { Value = "Боевик" } },
                Countries = new List<Country> { new Country { Value = "США" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 2", Genres = new List<Genre> { new Genre { Value = "Комедия" } },
                Countries = new List<Country> { new Country { Value = "Франция" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 3", Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Иран" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync(null, "Франция", CancellationToken.None);

        Assert.NotNull(result);
        result.Count.Should().Be(2);
        result.Should().Contain(m => m.NameRu == "Фильм 2");
        result.Should().Contain(m => m.NameRu == "Фильм 3");
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnMoviesByGenresAndCountries_WhenBothFiltersProvided()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 1", Genres = new List<Genre> { new Genre { Value = "Боевик" } },
                Countries = new List<Country> { new Country { Value = "США" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 2", Genres = new List<Genre> { new Genre { Value = "Комедия" } },
                Countries = new List<Country> { new Country { Value = "Франция" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 3", Genres = new List<Genre> { new Genre { Value = "Триллер" }, new Genre { Value = "Комедия" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Иран" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 4", Genres = new List<Genre> { new Genre { Value = "Ужасы" } },
                Countries = new List<Country> { new Country { Value = "США" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync("Боевик", "США", CancellationToken.None);

        Assert.NotNull(result);
        result.Count.Should().Be(1);
        result.Should().Contain(m => m.NameRu == "Фильм 1");
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnEmptyList_WhenNoMoviesMatchFilters()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 1", Genres = new List<Genre> { new Genre { Value = "Боевик" } },
                Countries = new List<Country> { new Country { Value = "США" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 2", Genres = new List<Genre> { new Genre { Value = "Комедия" } },
                Countries = new List<Country> { new Country { Value = "Франция" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "Фильм 3", Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Иран" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync("Драмма", "Турция", CancellationToken.None);

        Assert.Empty(result);
    }
}