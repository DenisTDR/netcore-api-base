using System;
using API.Base.Web.Base.Auth.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Database
{
    public class BaseDbContext : IdentityDbContext<User, Role, string>
    {
        public static Action<ModelBuilder> ConfigureBuilder;

        public BaseDbContext(DbContextOptions options) : base(options)
        {
//            Console.WriteLine("BaseDbContext ctor");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureBuilder(modelBuilder);
            modelBuilder.Entity<User>().ToTable("AuthUser");
            modelBuilder.Entity<Role>().ToTable("AuthRole");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AuthUserRole");
        }
    }
}