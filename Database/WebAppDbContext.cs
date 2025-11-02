using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Database.Tables;

namespace WebApp.Database;

public sealed class WebAppDbContext : IdentityDbContext<AppUser>
{
    public WebAppDbContext(DbContextOptions<WebAppDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebAppDbContext).Assembly);
    }

    #region Tables
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppIdentityRole> AppIdentityRoles { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<SeasonWeek> SeasonWeeks { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameSummary> GameSummaries { get; set; }
    public DbSet<GamePrediction> GamePredictions { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Division> Divisions { get; set; }
    public DbSet<Conference> Conferences { get; set; }
    public DbSet<BackgroundJobLog> BackgroundJobLogs { get; set; }
    public DbSet<InviteCode> InviteCodes { get; set; }
    #endregion
}
