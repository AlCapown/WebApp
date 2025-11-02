using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Database.Tables;

[Table(nameof(GameSummary))]
public class GameSummary
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GameSummaryId { get; set; }

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

internal sealed class GameSummaryConfiguration : IEntityTypeConfiguration<GameSummary>
{
    public void Configure(EntityTypeBuilder<GameSummary> builder)
    {
        builder
            .HasIndex(p => p.GameId)
            .IsUnique();

        builder
            .HasOne(p => p.Game)
            .WithOne(p => p.GameAISummary)
            .HasForeignKey<GameSummary>(p => p.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(p => p.DateCreated)
            .HasDefaultValueSql(SqlServerFunctions.SYS_DATETIME_OFFSET);
    }
}