using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApp.Database.Tables;

[Table(nameof(Conference))]
public class Conference
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ConferenceId { get; set; }

    [Required]
    [MaxLength(30)]
    public string ConferenceName { get; set; }

    [Required]
    [MaxLength(3)]
    public string ConferenceShortName { get; set; }

    #region Navigation Props
    public virtual ICollection<Division> Divisions { get; set; }
    #endregion
}

public class FootballConferenceMap : BaseEntityMap<Conference>
{
    protected override void InternalMap(EntityTypeBuilder<Conference> builder)
    {
        builder
            .HasData(ConferenceSeeds);
    }

    private static readonly Conference[] ConferenceSeeds =
    [
        new Conference
        {
            ConferenceId = 1,
            ConferenceName = "National Football Conference",
            ConferenceShortName = "NFC"
        },
        new Conference
        {
            ConferenceId = 2,
            ConferenceName = "American Football Conference",
            ConferenceShortName = "AFC"
        }
    ];
}
