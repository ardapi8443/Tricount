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
        SeedData(modelBuilder);

    }
    
            private static void SeedData(ModelBuilder modelBuilder) {


            User Boris = new User("Boris", "3D4AEC0A9B43782133B8120B2FDD8C6104ABB513FE0CDCD0D1D4D791AA42E338:C217604FDAEA7291C7BA5D1D525815E4:100000:SHA256", "boverhaegen@epfc.eu");
            User Benoît = new User("Benoît", "9E58D87797C6795D294E6762B6C05116D075BC18445AD4078C25674809DB57EF:C91E0B85B7264877C0424D52494D6296:100000:SHA256", "bepenelle@epfc.eu");
            User Xavier = new User("Xavier", "5B979AB86EC73B0996F439D0BC3947ECCFA0A41310C77533EA36CB409DBB1243:0CF43009110DE4B4AA6D4E749F622755:100000:SHA256", "xapigeolet@epfc.eu");
            User Marc = new User("Marc", "955F147CE3473774E35EE58F4233AA84AE9118C6ECD4699DD788B8D588238034:5514D1DD0A97E9BA7FE4C0B5A4E89351:100000:SHA256", "mamichel@epfc.eu");
            User Admin = new User("Administrator", "C9949A02A5DFBE50F1DA289DC162E3C97443AB09CE6F6EB1FD0C9D51B5241BBD:5533437973C5BC6459DB687CA5BDE76C:100000:SHA256", "admin@epfc.eu");

            modelBuilder.Entity<User>().HasData(Boris, Benoît, Xavier, Marc, Admin);

            Tricount tricount1 = new Tricount("Gers 2023", "", 1, new DateTime(2023, 10, 10, 18, 42, 24));
            Tricount tricount2 = new Tricount("Resto badminton", "", 1, new DateTime(2023, 10, 10, 19, 25, 10));
            Tricount tricount4 = new Tricount("Vacances", "A la mer du nord", 1, new DateTime(2023, 10, 10, 19, 31, 09));
            // Tricount tricount5 = new Tricount("Grosse virée", "A Torremolinos", 2, new DateTime(2023, 18, 15, 10, 00, 0));
            Tricount tricount6 = new Tricount("Torhout Werchter", "Memorabile", 3, new DateTime(2023, 07, 02, 18, 30, 12));

            modelBuilder.Entity<Tricount>().HasData(tricount1, tricount2, tricount4, tricount6);

            //Subscription(int User, int Tricount)

            Subscription subscription01 = new Subscription(1, 1);
            Subscription subscription02 = new Subscription(1, 2);
            Subscription subscription03 = new Subscription(1, 4);
            Subscription subscription04 = new Subscription(1, 6);
            Subscription subscription05 = new Subscription(2, 2);
            Subscription subscription06 = new Subscription(2, 4);
            Subscription subscription07 = new Subscription(2, 5);
            Subscription subscription08 = new Subscription(2, 6);
            Subscription subscription09 = new Subscription(3, 4);
            Subscription subscription10 = new Subscription(3, 5);
            Subscription subscription11 = new Subscription(3, 6);
            Subscription subscription12 = new Subscription(4, 4);
            Subscription subscription13 = new Subscription(4, 5);
            Subscription subscription14 = new Subscription(4, 6);
            Subscription subscription15 = new Subscription(2, 5);

            modelBuilder.Entity<Subscription>().HasData(subscription01, subscription02, subscription03, subscription04, subscription05, subscription06, subscription07, subscription08, subscription09, subscription10, subscription11, subscription12, subscription13, subscription14,subscription15);
            
        }

    }
    