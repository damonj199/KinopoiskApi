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

    [Fact]
    public async Task AddMoviesAsync_ShouldAddIfThereIsNoMovie()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 111, NameRu = "����� 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 222, NameRu = "����� 2", PremiereRu = new DateOnly(2024, 11, 15) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 333, NameRu = "����� 3", PremiereRu = new DateOnly(2024, 12, 1) }
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var filmNew = new List<Movie> { new Movie { Id = Guid.NewGuid(), KinopoiskId = 123, NameRu = "������ �������", PremiereRu = new DateOnly(2024, 12, 31) } };

        //Actions
        var result = await repo.AddMoviesAsync(filmNew, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Should().Contain(m => m.KinopoiskId == 123);
    }

    [Fact]
    public async Task AddMoviesAsync_ShouldUpdateIfThereIsMovieAndAdd()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 111, NameRu = "����� 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 222, NameRu = "����� 2", PremiereRu = new DateOnly(2024, 11, 15) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 333, NameRu = "����� 3", PremiereRu = new DateOnly(2024, 12, 1) }
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var filmNew = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 222, NameRu = "������ �������", PremiereRu = new DateOnly(2024, 12, 31) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 111, NameRu = "C���� ������ ����� 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), KinopoiskId = 123, NameRu = "C���� �����", PremiereRu = new DateOnly(2022, 1, 1) },
        };

        //Actions
        var result = await repo.AddMoviesAsync(filmNew, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
        result.Should().Contain(m => m.PremiereRu == new DateOnly(2024, 12, 31));
        result.Should().Contain(m => m.NameRu == "������ �������");
        result.Should().Contain(m => m.KinopoiskId == 123);

    }

    [Fact]
    public async Task GetPremieresForMonthAsync_ShouldReturnMoviesWithinSpecifiedDateRange()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 1", PremiereRu = new DateOnly(2024, 11, 1) },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 2", PremiereRu = new DateOnly(2024, 11, 15) },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 3", PremiereRu = new DateOnly(2024, 12, 1) }
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
        result.Should().Contain(m => m.NameRu == "����� 1");
        result.Should().Contain(m => m.NameRu == "����� 2");
    }

    [Fact]
    public async Task SearchMoviesByNameAsync_ShouldReturnMoviesMatchingTitle()
    {
        //Arrange
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie{Id = Guid.NewGuid(), NameRu = "����� ������", NameEn = "Films One"},
            new Movie{Id = Guid.NewGuid(), NameRu = "����� ������", NameEn = "Films Two"},
            new Movie{Id = Guid.NewGuid(), NameRu = "������", NameEn = "Unrelated"},
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        //Actions
        var result = await repo.SearchMoviesByNameAsync("�����", CancellationToken.None);

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
            new Movie {Id = Guid.NewGuid(), Year = 2012, NameRu = "����� �� ����� 1", PremiereRu = new DateOnly(2014, 10, 4)},
            new Movie {Id = Guid.NewGuid(), Year = 2015, NameRu = "����� �� ����� 2", PremiereRu = new DateOnly(2024, 11, 7)},
            new Movie {Id = Guid.NewGuid(), Year = 2011, NameRu = "����� �� ����� 3", PremiereRu = new DateOnly(2014, 10, 4)},
            new Movie {Id = Guid.NewGuid(), Year = 2014, NameRu = "����� �� ����� 4", PremiereRu = new DateOnly(2015, 1, 14)},
            new Movie {Id = Guid.NewGuid(), Year = 2012, NameRu = "����� �� ����� 5", PremiereRu = new DateOnly(2017, 3, 24)},
            new Movie {Id = Guid.NewGuid(), Year = 2020, NameRu = "����� �� ����� 6", PremiereRu = new DateOnly(2014, 10, 14)},
            new Movie {Id = Guid.NewGuid(), Year = 2021, NameRu = "����� �� ����� 7", PremiereRu = new DateOnly(2019, 11, 4)},
            new Movie {Id = Guid.NewGuid(), Year = 2022, NameRu = "����� �� ����� 8", PremiereRu = new DateOnly(2014, 12, 5)},
            new Movie {Id = Guid.NewGuid(), Year = 2022, NameRu = "����� �� ����� 9", PremiereRu = new DateOnly(2015, 11, 4)}
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

        //Action

        //Assert
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
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 1", Genres = new List<Genre> { new Genre { Value = "������" } },
                Countries = new List<Country> { new Country { Value = "���" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 2", Genres = new List<Genre> { new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 3", Genres = new List<Genre> { new Genre { Value = "������" }, new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" }, new Country { Value = "����" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        //Action
        var result = await repo.GetMoviesByFilterAsync(null, "", CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        result.Count.Should().Be(3); // ������ ��������� ��� ������
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnMoviesByGenres_WhenGenresFilterProvided()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 1", Genres = new List<Genre> { new Genre { Value = "������" } },
                Countries = new List<Country> { new Country { Value = "���" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 2", Genres = new List<Genre> { new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 3", Genres = new List<Genre> { new Genre { Value = "������" }, new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" }, new Country { Value = "����" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync("������", "", CancellationToken.None);

        Assert.NotNull(result);
        result.Count.Should().Be(2);
        result.Should().Contain(m => m.NameRu == "����� 1");
        result.Should().Contain(m => m.NameRu == "����� 3");
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnMoviesByCountries_WhenCountriesFilterProvided()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 1", Genres = new List<Genre> { new Genre { Value = "������" } },
                Countries = new List<Country> { new Country { Value = "���" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 2", Genres = new List<Genre> { new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 3", Genres = new List<Genre> { new Genre { Value = "������" }, new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" }, new Country { Value = "����" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync(null, "�������", CancellationToken.None);

        Assert.NotNull(result);
        result.Count.Should().Be(2);
        result.Should().Contain(m => m.NameRu == "����� 2");
        result.Should().Contain(m => m.NameRu == "����� 3");
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnMoviesByGenresAndCountries_WhenBothFiltersProvided()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 1", Genres = new List<Genre> { new Genre { Value = "������" } },
                Countries = new List<Country> { new Country { Value = "���" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 2", Genres = new List<Genre> { new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 3", Genres = new List<Genre> { new Genre { Value = "������" }, new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" }, new Country { Value = "����" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 4", Genres = new List<Genre> { new Genre { Value = "�����" } },
                Countries = new List<Country> { new Country { Value = "���" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync("������", "���", CancellationToken.None);

        Assert.NotNull(result);
        result.Count.Should().Be(1);
        result.Should().Contain(m => m.NameRu == "����� 1");
    }

    [Fact]
    public async Task GetMoviesByFilterAsync_ShouldReturnEmptyList_WhenNoMoviesMatchFilters()
    {
        using var context = new KinopoiskDbContext(_dboptions);

        var movies = new List<Movie>
        {
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 1", Genres = new List<Genre> { new Genre { Value = "������" } },
                Countries = new List<Country> { new Country { Value = "���" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 2", Genres = new List<Genre> { new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" } } },
            new Movie { Id = Guid.NewGuid(), NameRu = "����� 3", Genres = new List<Genre> { new Genre { Value = "������" }, new Genre { Value = "�������" } },
                Countries = new List<Country> { new Country { Value = "�������" }, new Country { Value = "����" } } },
        };
        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        var repo = new MoviesRepository(context);

        var result = await repo.GetMoviesByFilterAsync("������", "�������", CancellationToken.None);

        Assert.Empty(result); //��� ������� � ����� �������� � ������, ������ ���� �����
    }
}