using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;
using System.Configuration;

namespace prbd_2324_g01.Model;

public class PridContext : DbContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        /*
         * SQLite
         */

        // var connectionString = ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString;
        // optionsBuilder.UseSqlite(connectionString);

        /*
         * SQL Server
         */

        var connectionString = ConfigurationManager.ConnectionStrings["MsSqlConnectionString"].ConnectionString;
        optionsBuilder.UseSqlServer(connectionString);

        ConfigureOptions(optionsBuilder);
    }

    private static void ConfigureOptions(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseLazyLoadingProxies()
            .LogTo(Console.WriteLine, LogLevel.Information) // permet de visualiser les requêtes SQL générées par LINQ
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors() // attention : ralentit les requêtes
            ;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Template>()
            .HasMany(t =>t.TemplateItems)
            .WithOne(ti => ti.TemplateFromTemplateItem)
            .OnDelete(DeleteBehavior.ClientCascade);

        // à voir...
        //modelBuilder.Entity<User>()
            

    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Tricount> Tricounts => Set<Tricounts>();
    public DbSet<Subscription> Users => Set<Subscription>();
    public DbSet<Operation> Users => Set<Operation>();
    public DbSet<Repartition> Users => Set<Repartition>();
    public DbSet<Template> Users => Set<Template>();
    public DbSet<TemplateItem> Users => Set<TemplateItem>();
}