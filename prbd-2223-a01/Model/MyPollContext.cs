using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;

namespace MyPoll.Model;

public class MyPollContext : DbContextBase {
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder
            .UseSqlite("Data Source=prbd-2223-a01.db")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseLazyLoadingProxies(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Name = "ben" },
            new User { UserId = 2, Name = "bruno" }
            );
    }
}
