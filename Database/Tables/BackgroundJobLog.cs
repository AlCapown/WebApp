using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Database.Constants;

namespace WebApp.Database.Tables;

[Table(nameof(BackgroundJobLog))]
public class BackgroundJobLog
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LogId { get; set; }

    [Required]
    [MaxLength(500)]
    public string BackgroundJobName { get; set; }

    public bool IsSuccess { get; set; }
  
    [Column(TypeName = SQLServerTypes.DATETIME_OFFSET_ZERO)]
    public DateTimeOffset? Started { get; set; }

    [Column(TypeName = SQLServerTypes.DATETIME_OFFSET_ZERO)]
    public DateTimeOffset? Ended { get; set; }

    [Column(TypeName = SQLServerTypes.NVARCHAR_MAX)]
    public string ErrorsJson { get; set; }
}

internal sealed class BackgroundJobLogConfiguration : IEntityTypeConfiguration<BackgroundJobLog>
{
    public void Configure(EntityTypeBuilder<BackgroundJobLog> builder)
    {
    }
}
