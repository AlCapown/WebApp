using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Database.Tables;

[Table(nameof(Game))]
public class Game
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GameId { get; set; }

    [Required]
    public int SeasonWeekId { get; set; }

    [Column(TypeName = SQLServerTypes.DATETIME_OFFSET_ZERO)]
    public DateTimeOffset? StartsOn { get; set; }

    [Required]
    public int HomeTeamId { get; set; }

    [Required]
    public int HomeTeamScore { get; set; }

    [Required]
    public int AwayTeamId { get; set; }

    [Required]
    public int AwayTeamScore { get; set; }

    public int Quarter { get; set; }

    [MaxLength(20)]
    public string ClockTime { get; set; }

    [Required]
    public bool IsComplete { get; set; }


    #region Navigation Props
    public virtual SeasonWeek SeasonWeek { get; set; }
    public virtual Team HomeTeam { get; set; }
    public virtual Team AwayTeam { get; set; }
    public virtual ICollection<GamePrediction> GamePredictions { get; set; }
    #endregion
}

public class GameMap : BaseEntityMap<Game>
{
    protected override void InternalMap(EntityTypeBuilder<Game> builder)
    {
        builder
            .HasOne(p => p.SeasonWeek)
            .WithMany(p => p.Games)
            .HasForeignKey(p => p.SeasonWeekId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.HomeTeam)
            .WithMany(p => p.HomeGames)
            .HasForeignKey(p => p.HomeTeamId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder
            .HasOne(p => p.AwayTeam)
            .WithMany(p => p.AwayGames)
            .HasForeignKey(p => p.AwayTeamId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder
            .HasIndex(p => new
            {
                p.SeasonWeekId,
                p.HomeTeamId,
                p.AwayTeamId
            })
            .IsUnique();

        builder
            .HasIndex(p => p.SeasonWeekId);

        builder
            .Property(p => p.StartsOn)
            .HasDefaultValueSql(SqlServerFunctions.SYS_DATETIME_OFFSET);

        builder
            .Property(p => p.HomeTeamScore)
            .HasDefaultValue(0);

        builder
            .Property(p => p.AwayTeamScore)
            .HasDefaultValue(0);

        builder
            .Property(p => p.IsComplete)
            .HasDefaultValue(false);
    }
}
