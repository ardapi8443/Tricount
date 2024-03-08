using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;
using System.Configuration;

namespace prbd_2324_g01.Model;

public class PridContext : DbContextBase {
    public static Model Context { get; private set; } = new Model();
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
            .WithOne(ti => ti.Template)
            .OnDelete(DeleteBehavior.ClientCascade);

        // à voir...
        //modelBuilder.Entity<User>()
            

    }

   
}