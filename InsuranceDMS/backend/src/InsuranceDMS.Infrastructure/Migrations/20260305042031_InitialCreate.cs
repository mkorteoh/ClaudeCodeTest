using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceDMS.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "Carriers",
                columns: table => new
                {
                    CarrierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarrierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NAIC = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carriers", x => x.CarrierId);
                });

            migrationBuilder.CreateTable(
                name: "LicenseTypes",
                columns: table => new
                {
                    LicenseTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseTypes", x => x.LicenseTypeId);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateCode = table.Column<string>(type: "CHAR(2)", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.StateCode);
                });

            migrationBuilder.CreateTable(
                name: "Agencies",
                columns: table => new
                {
                    AgencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgencyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NPN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AgencyTier = table.Column<byte>(type: "tinyint", nullable: false),
                    ParentAgencyId = table.Column<int>(type: "int", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StateCode = table.Column<string>(type: "CHAR(2)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    County = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMerged = table.Column<bool>(type: "bit", nullable: false),
                    MergedIntoId = table.Column<int>(type: "int", nullable: true),
                    MergedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.AgencyId);
                    table.ForeignKey(
                        name: "FK_Agencies_Agencies_MergedIntoId",
                        column: x => x.MergedIntoId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                    table.ForeignKey(
                        name: "FK_Agencies_Agencies_ParentAgencyId",
                        column: x => x.ParentAgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                    table.ForeignKey(
                        name: "FK_Agencies_States_StateCode",
                        column: x => x.StateCode,
                        principalTable: "States",
                        principalColumn: "StateCode");
                });

            migrationBuilder.CreateTable(
                name: "Mergers",
                columns: table => new
                {
                    MergerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurvivingAgencyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InitiatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mergers", x => x.MergerId);
                    table.ForeignKey(
                        name: "FK_Mergers_Agencies_SurvivingAgencyId",
                        column: x => x.SurvivingAgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                });

            migrationBuilder.CreateTable(
                name: "Personnel",
                columns: table => new
                {
                    PersonnelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PersonnelType = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.PersonnelId);
                    table.ForeignKey(
                        name: "FK_Personnel_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                });

            migrationBuilder.CreateTable(
                name: "EntityLineages",
                columns: table => new
                {
                    LineageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MergerId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceEntityId = table.Column<int>(type: "int", nullable: false),
                    TargetEntityId = table.Column<int>(type: "int", nullable: true),
                    SourceAgencyId = table.Column<int>(type: "int", nullable: false),
                    TargetAgencyId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityLineages", x => x.LineageId);
                    table.ForeignKey(
                        name: "FK_EntityLineages_Agencies_SourceAgencyId",
                        column: x => x.SourceAgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                    table.ForeignKey(
                        name: "FK_EntityLineages_Agencies_TargetAgencyId",
                        column: x => x.TargetAgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                    table.ForeignKey(
                        name: "FK_EntityLineages_Mergers_MergerId",
                        column: x => x.MergerId,
                        principalTable: "Mergers",
                        principalColumn: "MergerId");
                });

            migrationBuilder.CreateTable(
                name: "MergerParticipants",
                columns: table => new
                {
                    ParticipantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MergerId = table.Column<int>(type: "int", nullable: false),
                    AbsorbedAgencyId = table.Column<int>(type: "int", nullable: false),
                    PersonnelTransferred = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MergerParticipants", x => x.ParticipantId);
                    table.ForeignKey(
                        name: "FK_MergerParticipants_Agencies_AbsorbedAgencyId",
                        column: x => x.AbsorbedAgencyId,
                        principalTable: "Agencies",
                        principalColumn: "AgencyId");
                    table.ForeignKey(
                        name: "FK_MergerParticipants_Mergers_MergerId",
                        column: x => x.MergerId,
                        principalTable: "Mergers",
                        principalColumn: "MergerId");
                });

            migrationBuilder.CreateTable(
                name: "Producers",
                columns: table => new
                {
                    ProducerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    NPN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ResidentState = table.Column<string>(type: "CHAR(2)", nullable: true),
                    SSNLast4 = table.Column<string>(type: "CHAR(4)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EOExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producers", x => x.ProducerId);
                    table.ForeignKey(
                        name: "FK_Producers_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "PersonnelId");
                    table.ForeignKey(
                        name: "FK_Producers_States_ResidentState",
                        column: x => x.ResidentState,
                        principalTable: "States",
                        principalColumn: "StateCode");
                });

            migrationBuilder.CreateTable(
                name: "CarrierAppointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProducerId = table.Column<int>(type: "int", nullable: false),
                    CarrierId = table.Column<int>(type: "int", nullable: false),
                    StateCode = table.Column<string>(type: "CHAR(2)", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppointmentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrierAppointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_CarrierAppointments_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "CarrierId");
                    table.ForeignKey(
                        name: "FK_CarrierAppointments_Producers_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "Producers",
                        principalColumn: "ProducerId");
                    table.ForeignKey(
                        name: "FK_CarrierAppointments_States_StateCode",
                        column: x => x.StateCode,
                        principalTable: "States",
                        principalColumn: "StateCode");
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    LicenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProducerId = table.Column<int>(type: "int", nullable: false),
                    StateCode = table.Column<string>(type: "CHAR(2)", nullable: false),
                    LicenseTypeId = table.Column<int>(type: "int", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.LicenseId);
                    table.ForeignKey(
                        name: "FK_Licenses_LicenseTypes_LicenseTypeId",
                        column: x => x.LicenseTypeId,
                        principalTable: "LicenseTypes",
                        principalColumn: "LicenseTypeId");
                    table.ForeignKey(
                        name: "FK_Licenses_Producers_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "Producers",
                        principalColumn: "ProducerId");
                    table.ForeignKey(
                        name: "FK_Licenses_States_StateCode",
                        column: x => x.StateCode,
                        principalTable: "States",
                        principalColumn: "StateCode");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_MergedIntoId",
                table: "Agencies",
                column: "MergedIntoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_NPN",
                table: "Agencies",
                column: "NPN",
                unique: true,
                filter: "[NPN] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_ParentAgencyId",
                table: "Agencies",
                column: "ParentAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_StateCode",
                table: "Agencies",
                column: "StateCode");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierAppointments_CarrierId",
                table: "CarrierAppointments",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierAppointments_ProducerId_CarrierId_StateCode",
                table: "CarrierAppointments",
                columns: new[] { "ProducerId", "CarrierId", "StateCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarrierAppointments_StateCode",
                table: "CarrierAppointments",
                column: "StateCode");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLineages_MergerId",
                table: "EntityLineages",
                column: "MergerId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLineages_SourceAgencyId",
                table: "EntityLineages",
                column: "SourceAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLineages_TargetAgencyId",
                table: "EntityLineages",
                column: "TargetAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_LicenseTypeId",
                table: "Licenses",
                column: "LicenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ProducerId",
                table: "Licenses",
                column: "ProducerId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_StateCode",
                table: "Licenses",
                column: "StateCode");

            migrationBuilder.CreateIndex(
                name: "IX_MergerParticipants_AbsorbedAgencyId",
                table: "MergerParticipants",
                column: "AbsorbedAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_MergerParticipants_MergerId_AbsorbedAgencyId",
                table: "MergerParticipants",
                columns: new[] { "MergerId", "AbsorbedAgencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mergers_SurvivingAgencyId",
                table: "Mergers",
                column: "SurvivingAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_AgencyId",
                table: "Personnel",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Producers_NPN",
                table: "Producers",
                column: "NPN",
                unique: true,
                filter: "[NPN] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Producers_PersonnelId",
                table: "Producers",
                column: "PersonnelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producers_ResidentState",
                table: "Producers",
                column: "ResidentState");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CarrierAppointments");

            migrationBuilder.DropTable(
                name: "EntityLineages");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "MergerParticipants");

            migrationBuilder.DropTable(
                name: "Carriers");

            migrationBuilder.DropTable(
                name: "LicenseTypes");

            migrationBuilder.DropTable(
                name: "Producers");

            migrationBuilder.DropTable(
                name: "Mergers");

            migrationBuilder.DropTable(
                name: "Personnel");

            migrationBuilder.DropTable(
                name: "Agencies");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
