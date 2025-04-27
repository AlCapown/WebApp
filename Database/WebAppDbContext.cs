using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApp.Database.Tables;

namespace WebApp.Database;

public class WebAppDbContext : IdentityDbContext<AppUser>
{
    private readonly IEnumerable<IEntityTypeMap> _mappings;

    public WebAppDbContext(DbContextOptions<WebAppDbContext> options, IEnumerable<IEntityTypeMap> mappings) 
        : base(options)
    {
        _mappings = mappings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply database mappings for each table that implements BaseEntityMap
        foreach (var mapping in _mappings)
        {
            mapping.Map(modelBuilder);
        }

        base.OnModelCreating(modelBuilder);
    }

    #region Tables
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppIdentityRole> AppIdentityRoles { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<SeasonWeek> SeasonWeeks { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GamePrediction> GamePredictions { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Division> Divisions { get; set; }
    public DbSet<Conference> Conferences { get; set; }
    public DbSet<BackgroundJobLog> BackgroundJobLogs { get; set; }
    public DbSet<InviteCode> InviteCodes { get; set; }
    #endregion
}
