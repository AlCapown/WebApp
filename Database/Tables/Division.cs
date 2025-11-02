using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Database.Tables;

[Table(nameof(Division))]
public class Division
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DivisionId { get; set; }

    [Required]
    [MaxLength(20)]
    public string DivisionName { get; set; }

    [Required]
    public int ConferenceId { get; set; }

    #region Navigation Props
    public virtual ICollection<Team> Teams { get; set; }
    public virtual Conference Conference { get; set; }
    #endregion
}

internal sealed class DivisionConfiguration : IEntityTypeConfiguration<Division>
{
    public void Configure(EntityTypeBuilder<Division> builder)
    {
        builder
            .HasOne(p => p.Conference)
            .WithMany(p => p.Divisions)
            .HasForeignKey(p => p.ConferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
             .HasData(DivisionSeeds);
    }

    private static readonly Division[] DivisionSeeds =
    [
        new Division { DivisionId = 1, DivisionName = "NFC NORTH", ConferenceId = 1 },
        new Division { DivisionId = 2, DivisionName = "NFC EAST", ConferenceId = 1 },
        new Division { DivisionId = 3, DivisionName = "NFC SOUTH", ConferenceId = 1 },
        new Division { DivisionId = 4, DivisionName = "NFC WEST", ConferenceId = 1 },
        new Division { DivisionId = 5, DivisionName = "AFC NORTH", ConferenceId = 2 },
        new Division { DivisionId = 6, DivisionName = "AFC EAST", ConferenceId = 2 },
        new Division { DivisionId = 7, DivisionName = "AFC SOUTH", ConferenceId = 2 },
        new Division { DivisionId = 8, DivisionName = "AFC WEST", ConferenceId = 2 },
    ];
}
