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
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // USER → POST (1:N)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // USER → OFFER (1:N)
        modelBuilder.Entity<Offer>()
            .HasOne(o => o.User)
            .WithMany(u => u.Offers)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // POST → OFFER (1:N)
        modelBuilder.Entity<Offer>()
            .HasOne(o => o.Post)
            .WithMany(p => p.Offers)
            .HasForeignKey(o => o.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // POST → CATEGORY (N:1)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // POST → MEDIA (1:N)
        modelBuilder.Entity<PostMedia>()
            .HasOne(m => m.Post)
            .WithMany(p => p.Media)
            .HasForeignKey(m => m.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // CATEGORY → SELF (Parent-Child)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // UNIQUE: Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // UNIQUE: Phone
        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.Post)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.PostId })
            .IsUnique();

        // REVIEW RELATIONSHIPS
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Reviewer)
            .WithMany(u => u.ReviewsGiven)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.TargetUser)
            .WithMany(u => u.ReviewsReceived)
            .HasForeignKey(r => r.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}