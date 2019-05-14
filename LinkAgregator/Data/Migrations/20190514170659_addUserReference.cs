using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LinkAgregator.Data.Migrations
{
    public partial class addUserReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "userId",
                table: "Links",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "Links");
        }
    }
}
