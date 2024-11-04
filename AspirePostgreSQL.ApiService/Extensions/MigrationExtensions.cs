using AspirePostgreSQL.Database;
using Microsoft.EntityFrameworkCore;

namespace AspirePostgreSQL.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int retryCount = 5; // Number of times to retry
        int delayInMilliseconds = 2000; // Delay between retries in milliseconds

        for (int attempt = 0; attempt < retryCount; attempt++)
        {
            try
            {
                dbContext.Database.Migrate();
                break; // Exit loop if migration is successful
            }
            catch (Npgsql.NpgsqlException ex)
            {
                if (attempt == retryCount - 1) // Last attempt, rethrow the exception
                {
                    throw;
                }

                Console.WriteLine($"Migration attempt {attempt + 1} failed: {ex.Message}");
                Thread.Sleep(delayInMilliseconds); // Wait before retrying
            }
        }
    }
}
