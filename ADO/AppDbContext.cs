using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ADO;

public class AppDbContext: DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(new ConfigurationBuilder()
                                        .AddJsonFile("connection.json")
                                        .Build()
                                        .GetConnectionString("DefaultConnection")
        );
    }
}