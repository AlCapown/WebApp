using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Database.Tables;

public class AppUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    #region Navigation Props
    public virtual ICollection<GamePrediction> GamePredictions { get; set; }
    #endregion
}
