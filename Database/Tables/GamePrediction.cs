using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Database.Tables;

[Table(nameof(GamePrediction))]
public class GamePrediction
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GamePredictionId { get; set; }

    [Required]
    public int GameId { get; set; }

    [Required]
    [MaxLength(450)]
    public string UserId { get; set; }

    [Required]
    public int HomeTeamScore { get; set; }

    [Required]
    public int AwayTeamScore { get; set; }

    [Column(TypeName = SQLServerTypes.DATETIME_OFFSET_ZERO)]
    public DateTimeOffset DateCreated { get; set; }

    #region Navigation Props
    public virtual Game Game { get; set; }
    public virtual AppUser AppUser { get; set; }
    #endregion
}

public class GamePredictionMap : BaseEntityMap<GamePrediction>
{
    protected override void InternalMap(EntityTypeBuilder<GamePrediction> builder)
    {
        builder
            .HasOne(p => p.Game)
            .WithMany(p => p.GamePredictions)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.AppUser)
            .WithMany(p => p.GamePredictions)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(p => new
            {
                p.GameId,
                p.UserId,
            })
            .IsUnique();

        builder
            .Property(p => p.HomeTeamScore)
            .HasDefaultValue(0);

        builder
            .Property(p => p.AwayTeamScore)
            .HasDefaultValue(0);

        builder
            .Property(p => p.DateCreated)
            .HasDefaultValueSql(SqlServerFunctions.SYS_DATETIME_OFFSET);
    }
}
