using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Database.Constants;

namespace WebApp.Database.Tables;

[Table(nameof(InviteCode))]
public class InviteCode
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Code { get; set; }

    [Required]
    [Column(TypeName = SQLServerTypes.DATETIME_OFFSET_ZERO)]
    public DateTimeOffset? Created { get; set; }

    [Required]
    [Column(TypeName = SQLServerTypes.DATETIME_OFFSET_ZERO)]
    public DateTimeOffset? Expires { get; set; }
}

internal sealed class InviteCodeConfiguration : IEntityTypeConfiguration<InviteCode>
{
    public void Configure(EntityTypeBuilder<InviteCode> builder)
    {
        builder
            .Property(p => p.Created)
            .HasDefaultValueSql(SqlServerFunctions.SYS_DATETIME_OFFSET);
    }
}

