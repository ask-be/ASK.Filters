using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ASK.Filters.Tests.EntityFramework;

public abstract class BaseEFTest
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ProductDbContext> _contextOptions;

    protected BaseEFTest(ITestOutputHelper output)
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<ProductDbContext>()
                          .UseSqlite(_connection)
                          .LogTo(output.WriteLine, LogLevel.Information)
                          .Options;

        using var context = GetContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.AddRange(Product.SampleValues);

        context.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    protected ProductDbContext GetContext() => new(_contextOptions);
}