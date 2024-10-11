namespace KinopoiskDB.Dal.Repository;

public class BaseRepository
{
    protected readonly KinopoiskDbContext _cxt;
    public BaseRepository(KinopoiskDbContext connectionString)
    {
        _cxt = connectionString;
    }
}
