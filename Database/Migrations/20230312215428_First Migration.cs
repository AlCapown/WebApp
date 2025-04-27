using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApp.Database.Migrations;

/// <inheritdoc />
public partial class FirstMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Conference",
            columns: table => new
            {
                ConferenceId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ConferenceName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                ConferenceShortName = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Conference", x => x.ConferenceId);
            });

        migrationBuilder.CreateTable(
            name: "DeviceCodes",
            columns: table => new
            {
                UserCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                DeviceCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                SubjectId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                ClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                Data = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeviceCodes", x => x.UserCode);
            });

        migrationBuilder.CreateTable(
            name: "Keys",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                Use = table.Column<string>(type: "nvarchar(450)", nullable: true),
                Algorithm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                IsX509Certificate = table.Column<bool>(type: "bit", nullable: false),
                DataProtected = table.Column<bool>(type: "bit", nullable: false),
                Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Keys", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PersistedGrants",
            columns: table => new
            {
                Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                SubjectId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                ClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                Expiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                ConsumedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                Data = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PersistedGrants", x => x.Key);
            });

        migrationBuilder.CreateTable(
            name: "Season",
            columns: table => new
            {
                SeasonId = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                SeasonStart = table.Column<DateTime>(type: "date", nullable: false),
                SeasonEnd = table.Column<DateTime>(type: "date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Season", x => x.SeasonId);
            });

        migrationBuilder.CreateTable(
            name: "SeasonWeekType",
            columns: table => new
            {
                SeasonWeekTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeasonWeekType", x => x.SeasonWeekTypeName);
            });

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Division",
            columns: table => new
            {
                DivisionId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DivisionName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                ConferenceId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Division", x => x.DivisionId);
                table.ForeignKey(
                    name: "FK_Division_Conference_ConferenceId",
                    column: x => x.ConferenceId,
                    principalTable: "Conference",
                    principalColumn: "ConferenceId",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "SeasonWeek",
            columns: table => new
            {
                SeasonWeekId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SeasonId = table.Column<int>(type: "int", nullable: false),
                Week = table.Column<int>(type: "int", nullable: false),
                SeasonWeekTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                WeekStart = table.Column<DateTime>(type: "date", nullable: false),
                WeekEnd = table.Column<DateTime>(type: "date", nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeasonWeek", x => x.SeasonWeekId);
                table.ForeignKey(
                    name: "FK_SeasonWeek_SeasonWeekType_SeasonWeekTypeName",
                    column: x => x.SeasonWeekTypeName,
                    principalTable: "SeasonWeekType",
                    principalColumn: "SeasonWeekTypeName",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SeasonWeek_Season_SeasonId",
                    column: x => x.SeasonId,
                    principalTable: "Season",
                    principalColumn: "SeasonId",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Team",
            columns: table => new
            {
                TeamId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TeamFullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                TeamName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                TeamShortName = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                BrandingLogo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                DivisionId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Team", x => x.TeamId);
                table.ForeignKey(
                    name: "FK_Team_Division_DivisionId",
                    column: x => x.DivisionId,
                    principalTable: "Division",
                    principalColumn: "DivisionId",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Game",
            columns: table => new
            {
                FootballGameId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SeasonWeekId = table.Column<int>(type: "int", nullable: false),
                StartsOn = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(0)", nullable: true, defaultValueSql: "SYSDATETIMEOFFSET()"),
                HomeTeamId = table.Column<int>(type: "int", nullable: false),
                HomeTeamScore = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                AwayTeamId = table.Column<int>(type: "int", nullable: false),
                AwayTeamScore = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                Quarter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClockTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsComplete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Game", x => x.FootballGameId);
                table.ForeignKey(
                    name: "FK_Game_SeasonWeek_SeasonWeekId",
                    column: x => x.SeasonWeekId,
                    principalTable: "SeasonWeek",
                    principalColumn: "SeasonWeekId",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Game_Team_AwayTeamId",
                    column: x => x.AwayTeamId,
                    principalTable: "Team",
                    principalColumn: "TeamId");
                table.ForeignKey(
                    name: "FK_Game_Team_HomeTeamId",
                    column: x => x.HomeTeamId,
                    principalTable: "Team",
                    principalColumn: "TeamId");
            });

        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "390eddd4-11b2-4af3-b10a-e84ae12084fd", null, "AppIdentityRole", "USER", "USER" },
                { "a7f02fd6-8d5d-40bd-8446-c6440d0929ee", null, "AppIdentityRole", "ADMIN", "ADMIN" }
            });

        migrationBuilder.InsertData(
            table: "Conference",
            columns: new[] { "ConferenceId", "ConferenceName", "ConferenceShortName" },
            values: new object[,]
            {
                { 1, "National Football Conference", "NFC" },
                { 2, "American Football Conference", "AFC" }
            });

        migrationBuilder.InsertData(
            table: "Season",
            columns: new[] { "SeasonId", "Description", "SeasonEnd", "SeasonStart" },
            values: new object[] { 2022, "2022-2023 Football Season", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

        migrationBuilder.InsertData(
            table: "SeasonWeekType",
            column: "SeasonWeekTypeName",
            values: new object[]
            {
                "OffSeason",
                "PostSeason",
                "Preseason",
                "RegularSeason"
            });

        migrationBuilder.InsertData(
            table: "Division",
            columns: new[] { "DivisionId", "ConferenceId", "DivisionName" },
            values: new object[,]
            {
                { 1, 1, "NFC NORTH" },
                { 2, 1, "NFC EAST" },
                { 3, 1, "NFC SOUTH" },
                { 4, 1, "NFC WEST" },
                { 5, 2, "AFC NORTH" },
                { 6, 2, "AFC EAST" },
                { 7, 2, "AFC SOUTH" },
                { 8, 2, "AFC WEST" }
            });

        migrationBuilder.InsertData(
            table: "Team",
            columns: new[] { "TeamId", "BrandingLogo", "DivisionId", "TeamFullName", "TeamName", "TeamShortName" },
            values: new object[,]
            {
                { 1, null, 4, "Arizona Cardinals", "Cardinals", "ARI" },
                { 2, null, 3, "Atlanta Falcons", "Falcons", "ATL" },
                { 3, null, 3, "Carolina Panthers", "Panthers", "CAR" },
                { 4, null, 1, "Chicago Bears", "Bears", "CHI" },
                { 5, null, 2, "Dallas Cowboys", "Cowboys", "DAL" },
                { 6, null, 1, "Detroit Lions", "Lions", "DET" },
                { 7, null, 1, "Green Bay Packers", "Packers", "GB" },
                { 8, null, 4, "Los Angeles Rams", "Rams", "LA" },
                { 9, null, 1, "Minnesota Vikings", "Vikings", "MIN" },
                { 10, null, 3, "New Orleans Saints", "Saints", "NO" },
                { 11, null, 2, "New York Giants", "Giants", "NYG" },
                { 12, null, 2, "Philadelphia Eagles", "Eagles", "PHI" },
                { 13, null, 4, "San Francisco 49ers", "49ers", "SF" },
                { 14, null, 4, "Seattle Seahawks", "Seahawks", "SEA" },
                { 15, null, 3, "Tampa Bay Buccaneers", "Buccaneers", "TB" },
                { 16, null, 2, "Washington Commanders", "Commanders", "WAS" },
                { 17, null, 5, "Baltimore Ravens", "Ravens", "BAL" },
                { 18, null, 6, "Buffalo Bills", "Bills", "BUF" },
                { 19, null, 7, "Indianapolis Colts", "Colts", "IND" },
                { 20, null, 5, "Cincinnati Bengals", "Bengals", "CIN" },
                { 21, null, 5, "Cleveland Browns", "Browns", "CLE" },
                { 22, null, 8, "Denver Broncos", "Broncos", "DEN" },
                { 23, null, 7, "Houston Texans", "Texans", "HOU" },
                { 24, null, 7, "Jacksonville Jaguars", "Jaguars", "JAX" },
                { 25, null, 8, "Kansas City Chiefs", "Chiefs", "KC" },
                { 26, null, 8, "Las Vegas Raiders", "Raiders", "LV" },
                { 27, null, 8, "Los Angeles Chargers", "Chargers", "LAC" },
                { 28, null, 6, "Miami Dolphins", "Dolphins", "MIA" },
                { 29, null, 6, "New England Patriots", "Patriots", "NE" },
                { 30, null, 6, "New York Jets", "Jets", "NYJ" },
                { 31, null, 5, "Pittsburgh Steelers", "Steelers", "PIT" },
                { 32, null, 7, "Tennessee Titans", "Titans", "TEN" }
            });

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_DeviceCodes_DeviceCode",
            table: "DeviceCodes",
            column: "DeviceCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_DeviceCodes_Expiration",
            table: "DeviceCodes",
            column: "Expiration");

        migrationBuilder.CreateIndex(
            name: "IX_Division_ConferenceId",
            table: "Division",
            column: "ConferenceId");

        migrationBuilder.CreateIndex(
            name: "IX_Game_AwayTeamId",
            table: "Game",
            column: "AwayTeamId");

        migrationBuilder.CreateIndex(
            name: "IX_Game_HomeTeamId",
            table: "Game",
            column: "HomeTeamId");

        migrationBuilder.CreateIndex(
            name: "IX_Game_SeasonWeekId",
            table: "Game",
            column: "SeasonWeekId");

        migrationBuilder.CreateIndex(
            name: "IX_Game_SeasonWeekId_HomeTeamId_AwayTeamId",
            table: "Game",
            columns: new[] { "SeasonWeekId", "HomeTeamId", "AwayTeamId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Keys_Use",
            table: "Keys",
            column: "Use");

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_ConsumedTime",
            table: "PersistedGrants",
            column: "ConsumedTime");

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_Expiration",
            table: "PersistedGrants",
            column: "Expiration");

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_SubjectId_ClientId_Type",
            table: "PersistedGrants",
            columns: new[] { "SubjectId", "ClientId", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_PersistedGrants_SubjectId_SessionId_Type",
            table: "PersistedGrants",
            columns: new[] { "SubjectId", "SessionId", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_SeasonWeek_SeasonId",
            table: "SeasonWeek",
            column: "SeasonId");

        migrationBuilder.CreateIndex(
            name: "IX_SeasonWeek_SeasonId_Week_SeasonWeekTypeName",
            table: "SeasonWeek",
            columns: new[] { "SeasonId", "Week", "SeasonWeekTypeName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_SeasonWeek_SeasonWeekTypeName",
            table: "SeasonWeek",
            column: "SeasonWeekTypeName");

        migrationBuilder.CreateIndex(
            name: "IX_Team_DivisionId",
            table: "Team",
            column: "DivisionId");

        migrationBuilder.CreateIndex(
            name: "IX_Team_TeamFullName",
            table: "Team",
            column: "TeamFullName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Team_TeamName",
            table: "Team",
            column: "TeamName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Team_TeamShortName",
            table: "Team",
            column: "TeamShortName",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "DeviceCodes");

        migrationBuilder.DropTable(
            name: "Game");

        migrationBuilder.DropTable(
            name: "Keys");

        migrationBuilder.DropTable(
            name: "PersistedGrants");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "SeasonWeek");

        migrationBuilder.DropTable(
            name: "Team");

        migrationBuilder.DropTable(
            name: "SeasonWeekType");

        migrationBuilder.DropTable(
            name: "Season");

        migrationBuilder.DropTable(
            name: "Division");

        migrationBuilder.DropTable(
            name: "Conference");
    }
}
