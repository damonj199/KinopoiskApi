using System.Data;
using System.Data.Common;

using KinopoiskDB.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KinopoiskDB.Dal.PostgreSQL;

public class KinopoiskDbContext : DbContext
{
    public KinopoiskDbContext(DbContextOptions<KinopoiskDbContext> options) : base(options) { }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }

    public async Task<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        var ctxTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        var transaction = ctxTransaction.GetDbTransaction();
        return transaction;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    ));
                }
            }
        }
    }
}
