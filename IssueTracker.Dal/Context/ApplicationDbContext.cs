using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Dal.Models;

namespace IssueTracker.Dal.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> tickets { get; set; }
    public DbSet<TicketComment> comments { get; set; }
    public DbSet<ChatEntity> chats { get; set; }
    public DbSet<UserMessage> messages { get; set; }
    public DbSet<Space> spaces { get; set; }
    public DbSet<WatchList> watchList { get; set; }
    public DbSet<ExecutionList> executionList { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Friends)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
            "UserFriends",
            u => u.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
            f => f.HasOne<ApplicationUser>().WithMany().HasForeignKey("FriendId"),
            j =>
            {
                j.HasKey("UserId", "FriendId");
                j.ToTable("UserFriends");
            });

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Subscriptions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
            "UserSubscriptions",
            u => u.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
            f => f.HasOne<ApplicationUser>().WithMany().HasForeignKey("FriendId"),
            j =>
            {
                j.HasKey("UserId", "FriendId");
                j.ToTable("UserSubscriptions");
            });

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.FriendRequests)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
            "UserFriendRequests",
            u => u.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
            f => f.HasOne<ApplicationUser>().WithMany().HasForeignKey("FriendId"),
            j =>
            {
                j.HasKey("UserId", "FriendId");
                j.ToTable("UserFriendRequests");
            });

        modelBuilder.Entity<ExecutionList>().HasKey(u => new { u.UserId, u.TicketId });

        modelBuilder.Entity<ExecutionList>()
            .HasOne(e => e.User)
            .WithMany(u => u.ExecutionList)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<ExecutionList>()
            .HasOne(e => e.Ticket)
            .WithMany(t => t.executionList)
            .HasForeignKey(e => e.TicketId);

        modelBuilder.Entity<WatchList>().HasKey(u => new { u.UserId, u.TicketId });

        modelBuilder.Entity<WatchList>()
            .HasOne(e => e.User)
            .WithMany(u => u.WatchList)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<WatchList>()
            .HasOne(e => e.Ticket)
            .WithMany(t => t.watchList)
            .HasForeignKey(e => e.TicketId);
    }
}