using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwaggerTest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataTypeTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Byte = table.Column<byte>(type: "tinyint", nullable: false),
                    SByte = table.Column<short>(type: "smallint", nullable: false),
                    USHort = table.Column<int>(type: "int", nullable: false),
                    Short = table.Column<short>(type: "smallint", nullable: false),
                    UInt = table.Column<long>(type: "bigint", nullable: false),
                    Int = table.Column<int>(type: "int", nullable: false),
                    ULong = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Long = table.Column<long>(type: "bigint", nullable: false),
                    Float = table.Column<float>(type: "real", nullable: false),
                    Double = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<double>(type: "float", nullable: false),
                    Decimal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxNvarchar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LimitNvarchar = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    GenderEnum = table.Column<int>(type: "int", nullable: false),
                    GenderByteEnum = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypeTables", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataTypeTables");
        }
    }
}
