using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Database.Tables;

[Table(nameof(Season))]
public class Season
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int SeasonId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Description { get; set; }

    public DateOnly SeasonStart { get; set; }

    public DateOnly SeasonEnd { get; set; }

    #region Navigation Props
    public virtual ICollection<SeasonWeek> SeasonWeeks { get; set; }
    #endregion
}

public class SeasonMap : BaseEntityMap<Season>
{
    protected override void InternalMap(EntityTypeBuilder<Season> builder)
    {
        builder
            .HasData(SeasonSeeds);
    }

    private static readonly Season[] SeasonSeeds =
    [
        new Season
        {
            SeasonId = 2022,
            Description = "2022-2023 Football Season"
        },
    ];
}