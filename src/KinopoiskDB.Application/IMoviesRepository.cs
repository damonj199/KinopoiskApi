﻿using KinopoiskDB.Core.Enum;
using KinopoiskDB.Core.Models;

namespace KinopoiskDB.Application;

public interface IMoviesRepository
{
    Task<List<Movie>> AddMoviesAsync(List<Movie> movies, CancellationToken cancellationToken);
    Task<IReadOnlyList<Movie>> GetMoviesByFilterAsync(MovieRequest mReq, CancellationToken cancellationToken);
    Task<List<Movie>> GetPremieresAsync(int year, Month month, CancellationToken cancellationToken);
    Task<List<Movie>> SearchMoviesByNameAsync(string title, CancellationToken cancellationToken);
}
