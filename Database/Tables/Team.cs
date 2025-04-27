using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Database.Tables;

/* 
 * TODO: Team locations can change like the OAK Raiders (now Las Vegas) that has effect on their name. 
 *
 * Current Idea: Should create a team Meta data table with all of the information 
 * that can change. This table should just have TeamId and a FK to a row in this table
 * that points to the current active entry. This keeps TeamIds the same even if their name
 * or city changes. 
 * 
 * Research: How do we handle division or conference changes too?
 */

[Table(nameof(Team))]
public class Team
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TeamId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TeamFullName { get; set; }

    [Required]
    [MaxLength(50)]
    public string TeamName { get; set; }

    [Required]
    [MaxLength(3)]
    public string Abbreviation { get; set; }

    [MaxLength(255)]
    public string BrandingLogo { get; set; }

    [Required]
    public int DivisionId { get; set; }

    #region Navigation Props
    public virtual ICollection<Game> HomeGames { get; set; }
    public virtual ICollection<Game> AwayGames { get; set; }
    public virtual Division Division { get; set; }
    #endregion
}

public class TeamMap : BaseEntityMap<Team>
{
    protected override void InternalMap(EntityTypeBuilder<Team> builder)
    {
        builder
            .HasOne(p => p.Division)
            .WithMany(p => p.Teams)
            .HasForeignKey(p => p.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(p => p.TeamFullName)
            .IsUnique();

        builder
            .HasIndex(p => p.TeamName)
            .IsUnique();

        builder
            .HasIndex(p => p.Abbreviation)
            .IsUnique();

        builder
            .HasData(TeamSeeds);
    }

    /// <summary>
    /// List of all NFL Teams
    /// </summary>
    private static readonly Team[] TeamSeeds =
    [
        new Team
        {
            TeamId = 1,
            TeamFullName = "Arizona Cardinals",
            TeamName = "Cardinals",
            Abbreviation = "ARI",
            BrandingLogo = null,
            DivisionId = 4
        },
        new Team
        {
            TeamId = 2,
            TeamFullName = "Atlanta Falcons",
            TeamName = "Falcons",
            Abbreviation = "ATL",
            BrandingLogo = null,
            DivisionId = 3
        },
        new Team
        {
            TeamId = 3,
            TeamFullName = "Carolina Panthers",
            TeamName = "Panthers",
            Abbreviation = "CAR",
            BrandingLogo = null,
            DivisionId = 3
        },
        new Team
        {
            TeamId = 4,
            TeamFullName = "Chicago Bears",
            TeamName = "Bears",
            Abbreviation = "CHI",
            BrandingLogo = null,
            DivisionId = 1
        },
        new Team
        {
            TeamId = 5,
            TeamFullName = "Dallas Cowboys",
            TeamName = "Cowboys",
            Abbreviation = "DAL",
            BrandingLogo = null,
            DivisionId = 2
        },
        new Team
        {
            TeamId = 6,
            TeamFullName = "Detroit Lions",
            TeamName = "Lions",
            Abbreviation = "DET",
            BrandingLogo = null,
            DivisionId = 1
        },
        new Team
        {
            TeamId = 7,
            TeamFullName = "Green Bay Packers",
            TeamName = "Packers",
            Abbreviation = "GB",
            BrandingLogo = null,
            DivisionId = 1
        },
        new Team
        {
            TeamId = 8,
            TeamFullName = "Los Angeles Rams",
            TeamName = "Rams",
            Abbreviation = "LAR",
            BrandingLogo = null,
            DivisionId = 4
        },
        new Team
        {
            TeamId = 9,
            TeamFullName = "Minnesota Vikings",
            TeamName = "Vikings",
            Abbreviation = "MIN",
            BrandingLogo = null,
            DivisionId = 1
        },
        new Team
        {
            TeamId = 10,
            TeamFullName = "New Orleans Saints",
            TeamName = "Saints",
            Abbreviation = "NO",
            BrandingLogo = null,
            DivisionId = 3
        },
        new Team
        {
            TeamId = 11,
            TeamFullName = "New York Giants",
            TeamName = "Giants",
            Abbreviation = "NYG",
            BrandingLogo = null,
            DivisionId = 2
        },
        new Team
        {
            TeamId = 12,
            TeamFullName = "Philadelphia Eagles",
            TeamName = "Eagles",
            Abbreviation = "PHI",
            BrandingLogo = null,
            DivisionId = 2
        },
        new Team
        {
            TeamId = 13,
            TeamFullName = "San Francisco 49ers",
            TeamName = "49ers",
            Abbreviation = "SF",
            BrandingLogo = null,
            DivisionId = 4
        },
        new Team
        {
            TeamId = 14,
            TeamFullName = "Seattle Seahawks",
            TeamName = "Seahawks",
            Abbreviation = "SEA",
            BrandingLogo = null,
            DivisionId = 4
        },
        new Team
        {
            TeamId = 15,
            TeamFullName = "Tampa Bay Buccaneers",
            TeamName = "Buccaneers",
            Abbreviation = "TB",
            BrandingLogo = null,
            DivisionId = 3
        },
        new Team
        {
            TeamId = 16,
            TeamFullName = "Washington Commanders",
            TeamName = "Commanders",
            Abbreviation = "WSH",
            BrandingLogo = null,
            DivisionId = 2
        },
        new Team
        {
            TeamId = 17,
            TeamFullName = "Baltimore Ravens",
            TeamName = "Ravens",
            Abbreviation = "BAL",
            BrandingLogo = null,
            DivisionId = 5
        },
        new Team
        {
            TeamId = 18,
            TeamFullName = "Buffalo Bills",
            TeamName = "Bills",
            Abbreviation = "BUF",
            BrandingLogo = null,
            DivisionId = 6
        },
        new Team
        {
            TeamId = 19,
            TeamFullName = "Indianapolis Colts",
            TeamName = "Colts",
            Abbreviation = "IND",
            BrandingLogo = null,
            DivisionId = 7
        },
        new Team
        {
            TeamId = 20,
            TeamFullName = "Cincinnati Bengals",
            TeamName = "Bengals",
            Abbreviation = "CIN",
            BrandingLogo = null,
            DivisionId = 5
        },
        new Team
        {
            TeamId = 21,
            TeamFullName = "Cleveland Browns",
            TeamName = "Browns",
            Abbreviation = "CLE",
            BrandingLogo = null,
            DivisionId = 5
        },
        new Team
        {
            TeamId = 22,
            TeamFullName = "Denver Broncos",
            TeamName = "Broncos",
            Abbreviation = "DEN",
            BrandingLogo = null,
            DivisionId = 8
        },
        new Team
        {
            TeamId = 23,
            TeamFullName = "Houston Texans",
            TeamName = "Texans",
            Abbreviation = "HOU",
            BrandingLogo = null,
            DivisionId = 7
        },
        new Team
        {
            TeamId = 24,
            TeamFullName = "Jacksonville Jaguars",
            TeamName = "Jaguars",
            Abbreviation = "JAX",
            BrandingLogo = null,
            DivisionId = 7
        },
        new Team
        {
            TeamId = 25,
            TeamFullName = "Kansas City Chiefs",
            TeamName = "Chiefs",
            Abbreviation = "KC",
            BrandingLogo = null,
            DivisionId = 8
        },
        new Team
        {
            TeamId = 26,
            TeamFullName = "Las Vegas Raiders",
            TeamName = "Raiders",
            Abbreviation = "LV",
            BrandingLogo = null,
            DivisionId = 8
        },
        new Team
        {
            TeamId = 27,
            TeamFullName = "Los Angeles Chargers",
            TeamName = "Chargers",
            Abbreviation = "LAC",
            BrandingLogo = null,
            DivisionId = 8
        },
        new Team
        {
            TeamId = 28,
            TeamFullName = "Miami Dolphins",
            TeamName = "Dolphins",
            Abbreviation = "MIA",
            BrandingLogo = null,
            DivisionId = 6
        },
        new Team
        {
            TeamId = 29,
            TeamFullName = "New England Patriots",
            TeamName = "Patriots",
            Abbreviation = "NE",
            BrandingLogo = null,
            DivisionId = 6
        },
        new Team
        {
            TeamId = 30,
            TeamFullName = "New York Jets",
            TeamName = "Jets",
            Abbreviation = "NYJ",
            BrandingLogo = null,
            DivisionId = 6
        },
        new Team
        {
            TeamId = 31,
            TeamFullName = "Pittsburgh Steelers",
            TeamName = "Steelers",
            Abbreviation = "PIT",
            BrandingLogo = null,
            DivisionId = 5
        },
        new Team
        {
            TeamId = 32,
            TeamFullName = "Tennessee Titans",
            TeamName = "Titans",
            Abbreviation = "TEN",
            BrandingLogo = null,
            DivisionId = 7
        }
    ];
}
