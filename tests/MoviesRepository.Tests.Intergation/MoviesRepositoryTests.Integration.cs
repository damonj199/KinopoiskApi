using KinopoiskDB.Core.Models;
using KinopoiskDB.Dal.PostgreSQL;
using KinopoiskDB.Dal.PostgreSQL.Repository;

using Microsoft.EntityFrameworkCore;

namespace MoviesRepositoryTests.Intergation;

public class MoviesRepositoryTests : IClassFixture<DataBaseFixture>
{
    private readonly KinopoiskDbContext _context;

    public MoviesRepositoryTests(DataBaseFixture fixture)
    {
        _context = fixture.Context;
    }

    [Fact(DisplayName = "Добавление нового фильма")]
    public async Task AddMoviesAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var repo = new MoviesRepository(_context);

        var movie = new List<Movie> { new Movie { Id = Guid.NewGuid(), KinopoiskId = 111, Year = 2024, NameRu = "Фильм 1+1", NameEn = "Movie 1+1", PremiereRu = new DateOnly(2024, 12, 1),
                Genres = new List<Genre> { new Genre { Value = "Боевик" }, new Genre { Value = "Триллер" } },
                Countries = new List<Country> { new Country { Value = "Франция" }, new Country { Value = "Испания" } } } };

        //Act
        await repo.AddMoviesAsync(movie, CancellationToken.None);

        //Assert
        var res = await _context.Movies.FirstOrDefaultAsync(x => x.NameEn == "Movie 1+1");
        Assert.NotNull(res);
        Assert.Equal("Movie 1+1", res.NameEn);
    }

    [Fact(DisplayName = "Покажет все фильмы в БД")]
    public async Task GetAllMoviesAsync_ShouldReturnAllMovies()
    {
        //Arrange
        var repo = new MoviesRepository(_context);

        //Act
        var movies = await repo.GetAllMoviesAsync(1, 4, KinopoiskDB.Core.Enum.SortableFields.KinopoiskId, KinopoiskDB.Core.Enum.SortOrder.ASC, CancellationToken.None);

        //Assert
        Assert.Equal(4, movies.Count);
    }

    [Fact(DisplayName = "Поиск фильма по азванию")]
    public async Task SearchMoviesByNameAsync_ShouldReturnMoviesMatchingTitle()
    {
        //Arrange
        var repo = new MoviesRepository(_context);

        //Act
        var films = await repo.SearchMoviesByNameAsync("Movies 11", CancellationToken.None);

        //Assert
        Assert.Empty(films);
    }
}
