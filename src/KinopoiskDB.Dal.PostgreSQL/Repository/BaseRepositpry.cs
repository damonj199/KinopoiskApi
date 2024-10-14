namespace KinopoiskDB.Dal.PostgreSQL.Repository;

public class BaseRepository
{
    protected readonly KinopoiskDbContext _cxt;
    public BaseRepository(KinopoiskDbContext connectionString)
    {
        _cxt = connectionString;
    }
}
