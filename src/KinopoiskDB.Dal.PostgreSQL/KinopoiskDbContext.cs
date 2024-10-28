using System.Data;
using System.Data.Common;

using KinopoiskDB.Core.Enum;
using KinopoiskDB.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
        modelBuilder.HasPostgresEnum<Month>()
            .Entity<Movie>(entity =>
            {
                entity.Property(e => e.Month).HasColumnType("month");
            });
    }
}
