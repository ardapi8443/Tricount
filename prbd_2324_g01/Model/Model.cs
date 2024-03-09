using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static prbd_2324_g01.Model.User;

namespace prbd_2324_g01.Model
{
    public class Model : PridContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);

            //  optionsBuilder.UseSqlite(@"Data Source=prbd_2324_g01.db")

            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=prbd_2324_g01.db")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            // Pour que le lazy loading fonctionne, les propriétés de navigation doivent être marquées comme public et virtual.
            .UseLazyLoadingProxies(true);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<User>().HasMany(User)
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Tricount> Tricounts => Set<Tricount>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Operation> Operations => Set<Operation>();
        public DbSet<Repartition> Repartitions => Set<Repartition>();
        public DbSet<Template> Templates => Set<Template>();
        public DbSet<TemplateItem> TemplateItems => Set<TemplateItem>();

    }

}
