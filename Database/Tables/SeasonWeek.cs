using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Common.Enums;

namespace WebApp.Database.Tables;

[Table(nameof(SeasonWeek))]
public class SeasonWeek
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SeasonWeekId { get; set; }

    [Required]
    public int SeasonId { get; set; }

    [Required]
    public int Week { get; set; }

    [Required]
    public WeekType SeasonWeekTypeName { get; set; }

    [Required]
    public DateOnly WeekStart { get; set; }

    [Required]
    public DateOnly WeekEnd { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }

    #region Navigation Props
    public virtual ICollection<Game> Games { get; set; }
    public virtual Season Season { get; set; }
    public virtual SeasonWeekType SeasonWeekType { get; set; }
    #endregion
}

internal sealed class SeasonWeekConfiguration : IEntityTypeConfiguration<SeasonWeek>
{
    public void Configure(EntityTypeBuilder<SeasonWeek> builder)
    {
        builder
            .Property(x => x.SeasonWeekTypeName)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder
            .HasOne(x => x.Season)
            .WithMany(x => x.SeasonWeeks)
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.SeasonWeekType)
            .WithMany(x => x.SeasonWeeks)
            .HasForeignKey(x => x.SeasonWeekTypeName)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.SeasonId, x.Week, x.SeasonWeekTypeName })
            .IsUnique();

        builder
            .HasIndex(x => x.SeasonId);
    }
}
