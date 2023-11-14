using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwaggerTest.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumToStringGenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenderStr",
                table: "DataTypeTables",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenderStr",
                table: "DataTypeTables");
        }
    }
}
