using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceDMS.Infrastructure.Migrations
{
    public partial class AddLocationEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_States_StateCode",
                table: "Agencies");

            migrationBuilder.DropIndex(
                name: "IX_Agencies_StateCode",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "County",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Agencies");

            migrationBuilder.AddColumn<string>(
                name: "MergerType",
                table: "Mergers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AbsorbedLocationId",
                table: "MergerParticipants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgencyLocations",
                columns: table => new
                {
                    AgencyLocationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AgencyId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsCorporateOffice = table.Column<bool>(type: "INTEGER", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    AddressLine1 = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    StateCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    County = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMerged = table.Column<bool>(type: "INTEGER", nullable: false),
                    OriginalAgencyId = table.Column<int>(type: "INTEGER", nullable: true),
                    AcquiredAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyLocations", x => x.AgencyLocationId);
                    table.ForeignKey(
                        name: "FK_AgencyLocations_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                    table.ForeignKey(
                        name: "FK_AgencyLocations_Agencies_OriginalAgencyId",
                        column: x => x.OriginalAgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                    table.ForeignKey(
                        name: "FK_AgencyLocations_States_StateCode",
                        column: x => x.StateCode,
                        principalTable: "States",
                        principalColumn: "StateCode");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelLocations",
                columns: table => new
                {
                    PersonnelLocationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonnelId = table.Column<int>(type: "INTEGER", nullable: false),
                    AgencyLocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelLocations", x => x.PersonnelLocationId);
                    table.ForeignKey(
                        name: "FK_PersonnelLocations_AgencyLocations_AgencyLocationId",
                        column: x => x.AgencyLocationId,
                        principalTable: "AgencyLocations",
                        principalColumn: "AgencyLocationId");
                    table.ForeignKey(
                        name: "FK_PersonnelLocations_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MergerParticipants_AbsorbedLocationId",
                table: "MergerParticipants",
                column: "AbsorbedLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyLocations_AgencyId_IsCorporateOffice",
                table: "AgencyLocations",
                columns: new[] { "AgencyId", "IsCorporateOffice" });

            migrationBuilder.CreateIndex(
                name: "IX_AgencyLocations_OriginalAgencyId",
                table: "AgencyLocations",
                column: "OriginalAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyLocations_StateCode",
                table: "AgencyLocations",
                column: "StateCode");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelLocations_AgencyLocationId",
                table: "PersonnelLocations",
                column: "AgencyLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelLocations_PersonnelId_AgencyLocationId",
                table: "PersonnelLocations",
                columns: new[] { "PersonnelId", "AgencyLocationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MergerParticipants_AgencyLocations_AbsorbedLocationId",
                table: "MergerParticipants",
                column: "AbsorbedLocationId",
                principalTable: "AgencyLocations",
                principalColumn: "AgencyLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MergerParticipants_AgencyLocations_AbsorbedLocationId",
                table: "MergerParticipants");

            migrationBuilder.DropTable(
                name: "PersonnelLocations");

            migrationBuilder.DropTable(
                name: "AgencyLocations");

            migrationBuilder.DropIndex(
                name: "IX_MergerParticipants_AbsorbedLocationId",
                table: "MergerParticipants");

            migrationBuilder.DropColumn(
                name: "MergerType",
                table: "Mergers");

            migrationBuilder.DropColumn(
                name: "AbsorbedLocationId",
                table: "MergerParticipants");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Agencies",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Agencies",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Agencies",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "Agencies",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Agencies",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Agencies",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "Agencies",
                type: "TEXT",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Agencies",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Agencies",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_StateCode",
                table: "Agencies",
                column: "StateCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_States_StateCode",
                table: "Agencies",
                column: "StateCode",
                principalTable: "States",
                principalColumn: "StateCode");
        }
    }
}
