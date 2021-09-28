using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;

namespace Sandbox.EFCore
{
    public class AppDbContext : DbContext
    {

        public DbSet<Fruit> Fruits { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fruit>().Property<int>("Id");
            modelBuilder.Entity<Address>().Property<int>("FruitId");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Database");
        }
    }

    public class Fruit
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string PostCode { get; set; }
    }

    public class FruitVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public string PostCode { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int orangeId = 0;
            using(var ctx = new AppDbContext())
            {
                var orange = new Fruit { Name = "Orange" };
                var apple = new Fruit { Name = "Apple" };

                ctx.Fruits.Add(orange);
                ctx.Fruits.Add(apple);

                orangeId = ctx.Entry(orange).Property<int>("Id").CurrentValue;
                var appleId = ctx.Entry(apple).Property<int>("Id").CurrentValue;

                ctx.SaveChanges();
            }
            using (var ctx = new AppDbContext())
            {
                var address = new Address { PostCode = "Moon" };

                ctx.Entry(address).Property<int>("FruitId").CurrentValue = orangeId;

                ctx.Addresses.Add(address);

                

                ctx.SaveChanges();
            }
            using (var ctx = new AppDbContext())
            {
                var fruit = ctx.Fruits
                    .Include(x => x.Address)
                    .Select(x => new FruitVm
                    {
                        Name = x.Name,
                        PostCode = x.Address.PostCode
                    })
                    .ToList();
            }
        }
    }
}
