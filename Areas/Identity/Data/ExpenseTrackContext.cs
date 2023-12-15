using ExpenseTrack.Areas.Identity.Data;
using ExpenseTrack.Models;
using ExpenseTrack.Models.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ExpenseTrack.Data;

public class ExpenseTrackContext : IdentityDbContext<User>
{

    public DbSet<UserInfo> UserInfo { get; set; }

    public ExpenseTrackContext(DbContextOptions<ExpenseTrackContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);


        builder.Entity<User>()
          .HasOne(u => u.UserInfo)
          .WithOne(ui => ui.User)
          .HasForeignKey<UserInfo>(ui => ui.UserId);


        builder.Entity<User>()
            .Property(e => e.firstName)
        .HasMaxLength(250);

        builder.Entity<User>()
            .Property(e => e.lastName)
            .HasMaxLength(250);
    }
}
