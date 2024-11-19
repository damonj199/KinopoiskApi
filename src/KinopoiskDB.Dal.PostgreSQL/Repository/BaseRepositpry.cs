namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public abstract class BaseRepository
{
    protected readonly KinopoiskDbContext Context;

    public BaseRepository(KinopoiskDbContext connectionString)
    {
        Context = connectionString;
    }
}
