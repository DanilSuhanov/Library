using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Library.Class;

#nullable disable

namespace Library
{
    public partial class antContext : DbContext
    {
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Book> Books { get; set; }
        public antContext()
        {
        }

        public antContext(DbContextOptions<antContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ant;Username=postgres;Password=kadjitnichegonekral");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
