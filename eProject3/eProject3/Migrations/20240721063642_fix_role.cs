using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eProject3.Migrations
{
    /// <inheritdoc />
    public partial class fix_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
