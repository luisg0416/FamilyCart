namespace FamilyCart.Infrastructure.Data;

using FamilyCart.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext <User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {   
    }
    public DbSet<Family> Families { get; set; }
    public DbSet<FamilyMember> FamilyMembers { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ListItem> ListItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Below are the different relationships defined
        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.HasOne(sl => sl.Family)
                .WithMany(f => f.ShoppingLists)
                .HasForeignKey(sl => sl.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sl => sl.Store)
                .WithMany(s => s.ShoppingLists)
                .HasForeignKey(sl => sl.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sl => sl.CreatedByUser)
                .WithMany(u => u.ShoppingLists)
                .HasForeignKey(sl => sl.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ListItem>(entity =>
        {
            entity.HasOne(li => li.ShoppingList)
                .WithMany(sl => sl.ListItems)
                .HasForeignKey(li => li.ShoppingListId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(li => li.Product)
                .WithMany(p => p.ListItems)
                .HasForeignKey(li => li.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(li => li.AddedByUser)
                .WithMany(u => u.ListItems)
                .HasForeignKey(li => li.AddedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasOne(fm => fm.User)
                .WithMany(u => u.FamilyMemberships)
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(fm => fm.Family)
                .WithMany(f => f.FamilyMembers)
                .HasForeignKey(fm => fm.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Family>(entity =>
        {
            entity.HasOne(f => f.CreatedByUser)
                .WithMany(u => u.FamiliesCreated)
                .HasForeignKey(f => f.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(p => p.Store)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Family)
            .WithMany(f => f.Products)
            .HasForeignKey(p => p.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);
        });
    }

}
