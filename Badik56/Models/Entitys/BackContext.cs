using Microsoft.EntityFrameworkCore;
using Badik56.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Entitys
{
    public class BackContext : DbContext
    {
        public DbSet<About> Abouts { get; set; }
        public DbSet<Baner> Baners { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Click> Clicks { get; set; }
        public DbSet<Coming> Comings { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Expend> Expends { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Rule> Rule { get; set; }
        public DbSet<Rules> Rules { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<IP> IPs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Image> Images { get; set; }

        public BackContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;Database=u0893839_BuDatabase;User=u0893_adminDB;Password=snRq40~6;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
