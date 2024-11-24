using KinopoiskDB.Dal.PostgreSQL;

using Microsoft.EntityFrameworkCore;

namespace MoviesRepository.Tests.Intergation;

public class DataBaseFixture : IDisposable
{
    public KinopoiskDbContext Context { get; private set; }

    public void Dispose()
    {
        Context.Database.CloseConnection();
        Context.Dispose();
    }
}
