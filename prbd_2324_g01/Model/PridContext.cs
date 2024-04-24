using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;
using System.Configuration;

namespace prbd_2324_g01.Model;

public class PridContext : DbContextBase {
    public static PridContext Context { get; private set; } = new PridContext();

    public DbSet<User> Users => Set<User>();
    public DbSet<Tricount> Tricounts => Set<Tricount>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Repartition> Repartitions => Set<Repartition>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<TemplateItem> TemplateItems => Set<TemplateItem>();
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

            // .LogTo(Console.WriteLine, LogLevel.Information) // permet de visualiser les requêtes SQL générées par LINQ

            .EnableSensitiveDataLogging()
            .EnableDetailedErrors() // attention : ralentit les requêtes
            ;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.TricountCreated)
            .WithOne(t => t.CreatorFromTricount)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.OperationsCreated)
            .WithOne(o => o.Initiator)
            .OnDelete(DeleteBehavior.ClientCascade);

        

        modelBuilder.Entity<User>()
            .HasDiscriminator(u => u.Role)
            .HasValue<User>(Role.Viewer)
            .HasValue<Administrator>(Role.Administrator);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Tricounts)
            .WithMany(t => t.Subscribers)
            .UsingEntity<Subscription>(
                right => right.HasOne(s => s.TricountFromSubscription).WithMany()
                    .HasForeignKey(nameof(Subscription.TricountId))
                    .OnDelete(DeleteBehavior.ClientCascade),
                left => left.HasOne(s => s.UserFromSubscription).WithMany()
                    .HasForeignKey(nameof(Subscription.UserId))
                    .OnDelete(DeleteBehavior.ClientCascade),
                    
                 joinEntity => {
                     joinEntity.ToTable("Subscription"); // Rename the join table if necessary
                     joinEntity.HasKey(s => new { s.TricountId, s.UserId });
                 });


        modelBuilder.Entity<Template>()
                    .HasMany(t => t.Users)
                    .WithMany(u => u.Templates)
                    .UsingEntity<TemplateItem>(
                        right => right.HasOne(ti => ti.UserFromTemplateItem).WithMany()
                            .HasForeignKey(nameof(TemplateItem.User))
                            .OnDelete(DeleteBehavior.ClientCascade),
                        left => left.HasOne(ti => ti.TemplateFromTemplateItem).WithMany()
                            .HasForeignKey(nameof(TemplateItem.Template))
                            .OnDelete(DeleteBehavior.ClientCascade),
                         joinEntity =>
                         {
                             joinEntity.HasKey(ti => new { ti.User, ti.Template });
                         });


        modelBuilder.Entity<Operation>()
            .HasMany(o => o.Users)
            .WithMany(u => u.Operations)
            .UsingEntity<Repartition>(
                right => right.HasOne(r => r.UserFromRepartition).WithMany()
                    .HasForeignKey(nameof(Repartition.UserId))
                    .OnDelete(DeleteBehavior.ClientCascade),
                left => left.HasOne(r => r.OperationFromRepartition).WithMany()
                     .HasForeignKey(nameof(Repartition.OperationId))
                     .OnDelete(DeleteBehavior.ClientCascade),
                joinEntity => {
                    joinEntity.HasKey(r => new { r.UserId, r.OperationId });
                });

        SeedData(modelBuilder);

    }
   
            private static void SeedData(ModelBuilder modelBuilder) {


            User Boris = new User(1, "Boris", "3D4AEC0A9B43782133B8120B2FDD8C6104ABB513FE0CDCD0D1D4D791AA42E338:C217604FDAEA7291C7BA5D1D525815E4:100000:SHA256", "boverhaegen@epfc.eu");
            User Benoît = new User(2, "Benoît", "9E58D87797C6795D294E6762B6C05116D075BC18445AD4078C25674809DB57EF:C91E0B85B7264877C0424D52494D6296:100000:SHA256", "bepenelle@epfc.eu");
            User Xavier = new User(3, "Xavier", "5B979AB86EC73B0996F439D0BC3947ECCFA0A41310C77533EA36CB409DBB1243:0CF43009110DE4B4AA6D4E749F622755:100000:SHA256", "xapigeolet@epfc.eu");
            User Marc = new User(4, "Marc", "955F147CE3473774E35EE58F4233AA84AE9118C6ECD4699DD788B8D588238034:5514D1DD0A97E9BA7FE4C0B5A4E89351:100000:SHA256", "mamichel@epfc.eu");
            Administrator Admin = new Administrator(5, "Administrator", "C9949A02A5DFBE50F1DA289DC162E3C97443AB09CE6F6EB1FD0C9D51B5241BBD:5533437973C5BC6459DB687CA5BDE76C:100000:SHA256", "admin@epfc.eu");

            modelBuilder.Entity<User>().HasData(Boris, Benoît, Xavier, Marc);
            modelBuilder.Entity<Administrator>().HasData(Admin);

            Tricount tricount1 = new Tricount(1, "Gers 2023", "", 1, new DateTime(2023, 10, 10, 18, 42, 24));
            Tricount tricount2 = new Tricount(2, "Resto badminton", "", 1, new DateTime(2023, 10, 10, 19, 25, 10));
            Tricount tricount4 = new Tricount(4, "Vacances", "A la mer du nord", 1, new DateTime(2023, 10, 10, 19, 31, 09));
            Tricount tricount5 = new Tricount(5, "Grosse virée", "A Torremolinos", 2, new DateTime(2023, 08, 15, 10, 00, 00));
            Tricount tricount6 = new Tricount(6, "Torhout Werchter", "Memorabile", 3, new DateTime(2023, 06, 02, 18, 30, 12));

            modelBuilder.Entity<Tricount>().HasData(tricount1, tricount2, tricount4,tricount5, tricount6);

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


        modelBuilder.Entity<Subscription>().HasData(subscription01, subscription02, subscription03, subscription04, subscription05, subscription06, subscription07, subscription08, subscription09, subscription10, subscription11, subscription12, subscription13, subscription14);


        //Operation(int id, string title, int tricouht, double amount ,datetime operation_date, int initiator)

        Operation operation01 = new Operation(1, "Colruyt", 4, 100.00, new DateTime(2023, 10, 13), 2);
        Operation operation02 = new Operation(2, "Plein essence", 4, 75.00, new DateTime(2023, 10, 13), 1);
        Operation operation03 = new Operation(3, "Grosses courses LIDL", 4, 212.47, new DateTime(2023, 10, 13), 3);
        Operation operation04 = new Operation(4, "Apéros", 4, 31.897456217, new DateTime(2023, 10, 13), 1);
        Operation operation05 = new Operation(5, "Boucherie", 4, 25.50, new DateTime(2023, 10, 26), 2);
        Operation operation06 = new Operation(6, "Loterie", 4, 35.00, new DateTime(2023, 10, 26), 1);
        Operation operation07 = new Operation(7, "Sangria", 5, 42.00, new DateTime(2023, 8, 16), 2);
        Operation operation08 = new Operation(8, "Jet Ski", 5, 250.00, new DateTime(2023, 10, 17), 3);
        Operation operation09 = new Operation(9, "PV parking", 5, 15.50, new DateTime(2023, 10, 16), 3);
        Operation operation10 = new Operation(10, "Tickets", 6, 220.00, new DateTime(2023, 06, 08), 1);
        Operation operation11 = new Operation(11, "Decathlon", 6, 199.99, new DateTime(2023, 07, 01), 2);
        
        modelBuilder.Entity<Operation>().HasData(operation01,operation02,operation03,operation04,operation05,operation06,operation07,operation08,operation09,operation10,operation11);
        
        //Repartition (int userId, int Operation, int Weight

        Repartition repartition1 = new Repartition(1, 1, 1);
        Repartition repartition2 = new Repartition(2, 1, 1);
        Repartition repartition3 = new Repartition(1, 2, 1);
        Repartition repartition4 = new Repartition(2, 2, 1);
        Repartition repartition5 = new Repartition(1, 3, 2);
        Repartition repartition6 = new Repartition(2, 3, 1);
        Repartition repartition7 = new Repartition(3, 3, 1);
        Repartition repartition8 = new Repartition(1, 4, 1);
        Repartition repartition9 = new Repartition(2, 4, 2);
        Repartition repartition10 = new Repartition(3, 4, 3);
        Repartition repartition11 = new Repartition(1, 5, 2);
        Repartition repartition12 = new Repartition(2, 5, 1);
        Repartition repartition13 = new Repartition(3, 5, 1);
        Repartition repartition14 = new Repartition(1, 6, 1);
        Repartition repartition15 = new Repartition(3, 6, 1);
        Repartition repartition16 = new Repartition(2, 7, 1);
        Repartition repartition17 = new Repartition(3, 7, 2);
        Repartition repartition18 = new Repartition(4, 7, 3);
        Repartition repartition19 = new Repartition(3, 8, 2);
        Repartition repartition20 = new Repartition(4, 8, 1);
        Repartition repartition21 = new Repartition(2, 9, 1);
        Repartition repartition22 = new Repartition(4, 9, 5);
        Repartition repartition23 = new Repartition(1, 10, 1);
        Repartition repartition24 = new Repartition(3,10,1);
        Repartition repartition25 = new Repartition(2,11,2);
        Repartition repartition26 = new Repartition(4,11,2);

        modelBuilder.Entity<Repartition>().HasData(
            repartition1, repartition2, repartition3, repartition4, repartition5, repartition6, repartition7, repartition8, repartition9, repartition10, 
            repartition11, repartition12, repartition13, repartition14, repartition15, repartition16, repartition17, repartition18, repartition19, repartition20,
            repartition21, repartition22, repartition23, repartition24, repartition25, repartition26);


        //Template(int TemplateId, string Title, int Tricount)
        Template template1 = new Template(1, "Boris paye double", 4);
        Template template2 = new Template(2, "Boris paye rien", 4);

        modelBuilder.Entity<Template>().HasData(
            template1, template2);
        

        //TemplateItem(int weight, int User, int Template)
        TemplateItem templateItem1 = new TemplateItem(2, 1, 1);
        TemplateItem templateItem2 = new TemplateItem(1, 2, 1);
        TemplateItem templateItem3 = new TemplateItem(1, 3, 1);
        TemplateItem templateItem4 = new TemplateItem(1, 1, 2);
        TemplateItem templateItem5 = new TemplateItem(1, 3, 2);

        modelBuilder.Entity<TemplateItem>().HasData(
            templateItem1, templateItem2, templateItem3, templateItem4, templateItem5);
    }

}
    