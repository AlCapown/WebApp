﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Common.Constants;

namespace WebApp.Database.Tables;

public class AppIdentityRole : IdentityRole
{
    #region Navigation Props
    #endregion
}

public class AppIdentityRoleMap : BaseEntityMap<AppIdentityRole>
{
    protected override void InternalMap(EntityTypeBuilder<AppIdentityRole> builder)
    {
        builder
            .HasData(IdentityRoleSeeds);
    }

    private static readonly AppIdentityRole[] IdentityRoleSeeds =
    [
        new AppIdentityRole
        {
            Id = AppRole.USER,
            Name = AppRole.USER,
            NormalizedName = AppRole.USER,
        },
        new AppIdentityRole
        {
            Id = AppRole.ADMIN,
            Name = AppRole.ADMIN,
            NormalizedName = AppRole.ADMIN
        }
    ];
}
