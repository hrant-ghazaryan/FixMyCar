using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PostMedia> PostMedia { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureRelations(modelBuilder);
    }

    private void ConfigureRelations(ModelBuilder modelBuilder)
    {
        // User → Post (1 to many)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User → Offer (1 to many)
        // Offer → User (NO CASCADE to avoid multiple cascade paths)
        modelBuilder.Entity<Offer>()
            .HasOne(o => o.User)
            .WithMany(u => u.Offers)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Offer → Post (CASCADE is OK)
        modelBuilder.Entity<Offer>()
            .HasOne(o => o.Post)
            .WithMany(p => p.Offers)
            .HasForeignKey(o => o.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Post → Offer (1 to many)
        modelBuilder.Entity<Offer>()
            .HasOne(o => o.User)
            .WithMany(u => u.Offers)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Post → Category (many to one)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Post → PostMedia (1 to many)
        modelBuilder.Entity<PostMedia>()
            .HasOne(m => m.Post)
            .WithMany(p => p.Media)
            .HasForeignKey(m => m.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category self relation (Parent-Child)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
