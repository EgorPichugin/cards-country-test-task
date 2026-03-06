using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DellinTerminalImporter.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "offices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CityCode = table.Column<int>(type: "integer", nullable: false),
                    Uuid = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    full_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WorkTime = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "phones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Primary = table.Column<bool>(type: "boolean", nullable: false),
                    OfficeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_phones_offices_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_offices_city_code",
                table: "offices",
                column: "CityCode");

            migrationBuilder.CreateIndex(
                name: "ix_offices_code",
                table: "offices",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "ix_offices_country_code",
                table: "offices",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "ix_offices_type",
                table: "offices",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "ix_phones_office_id",
                table: "phones",
                column: "OfficeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "phones");

            migrationBuilder.DropTable(
                name: "offices");
        }
    }
}
