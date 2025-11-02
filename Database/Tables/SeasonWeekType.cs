using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Common.Enums;

namespace WebApp.Database.Tables;

[Table(nameof(SeasonWeekType))]
public class SeasonWeekType
{
    [Key]
    [Required]
    public WeekType SeasonWeekTypeName { get; set; }

    #region Navigation Props
    public virtual ICollection<SeasonWeek> SeasonWeeks { get; set; }
    #endregion
}

internal sealed class SeasonWeekTypeConfiguration : IEntityTypeConfiguration<SeasonWeekType>
{
    public void Configure(EntityTypeBuilder<SeasonWeekType> builder)
    {
        builder
            .Property(e => e.SeasonWeekTypeName)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder
            .HasData(SeasonWeekTypeSeeds);
    }

    private static readonly SeasonWeekType[] SeasonWeekTypeSeeds =
    [
        new SeasonWeekType
        {
            SeasonWeekTypeName = WeekType.Preseason,
        },
        new SeasonWeekType
        {
            SeasonWeekTypeName = WeekType.RegularSeason,
        },
        new SeasonWeekType
        {
            SeasonWeekTypeName = WeekType.PostSeason,
        },
        new SeasonWeekType
        {
            SeasonWeekTypeName = WeekType.OffSeason,
        },
    ];
}
