using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Title)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Title)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            #region UserSeed
            modelBuilder.Entity<User>()
                .HasData(
                    new User { Id = 1, Username = "Batman", Password = "batsenha", Role = "manager" },
                    new User { Id = 2, Username = "Robin", Password = "batsenha", Role = "employee" }
                );
            #endregion
        }
    }
}