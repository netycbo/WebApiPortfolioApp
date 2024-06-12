﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApiPortfolioApp.Data.Entinities;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.Providers;

namespace WebApiPortfolioApp.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly UserResolverService _userResolverService;

        public AppDbContext(DbContextOptions<AppDbContext> options, UserResolverService userService) : base(options)
        {
            _userResolverService = userService;
        }

        public DbSet<SearchHistory> SearchHistory { get; set; }
        public DbSet<ProductSubscription> ProductSubscriptions { get; set; }
        
        public async Task<int> SaveChangesAsync()
        {
            var entries = ChangeTracker
                   .Entries()
                   .Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((AuditableEntity)entityEntry.Entity).LastModified = DateTime.UtcNow;
                ((AuditableEntity)entityEntry.Entity).LastModifiedBy = _userResolverService.GetUser();

                if (entityEntry.State == EntityState.Added)
                {
                    ((AuditableEntity)entityEntry.Entity).Created = DateTime.UtcNow;
                    ((AuditableEntity)entityEntry.Entity).CreatedBy = _userResolverService.GetUser();
                }
            }

            OnBeforeSaveChanges();

            return await base.SaveChangesAsync();
        }
        private void OnBeforeSaveChanges()
        {
            var subscribeProductEntries = ChangeTracker.Entries<ProductSubscription>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in subscribeProductEntries)
            {
                var user = Users.Find(entry.Entity.UserId);
                if (user != null)
                {
                    user.IsSubscribedToNewsLetter = true;
                    Entry(user).State = EntityState.Modified;
                }
            }
        }
    }
}
