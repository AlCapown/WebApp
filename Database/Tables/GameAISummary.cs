using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Database.Tables;

[Table(nameof(GameAISummary))]
public class GameAISummary
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GameAISummaryId { get; set; }

    [Required]
    public int GameId { get; set; }

    [Required]
    [MaxLength(int.MaxValue)]
    public string Summary { get; set; }

    [Required]
    [Column(TypeName = SQLServerTypes.DATETIME_OFFSET_ZERO)]
    public DateTimeOffset DateCreated { get; set; }

    #region Navigation Props
    public virtual Game Game { get; set; }
    #endregion
}

public class GameAISummaryMap : BaseEntityMap<GameAISummary>
{
    protected override void InternalMap(EntityTypeBuilder<GameAISummary> builder)
    {
        builder
            .HasIndex(p => p.GameId)
            .IsUnique();

        builder
            .HasOne(p => p.Game)
            .WithOne(p => p.GameAISummary)
            .HasForeignKey<GameAISummary>(p => p.GameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}