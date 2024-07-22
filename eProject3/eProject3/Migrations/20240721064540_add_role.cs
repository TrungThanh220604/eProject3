using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eProject3.Migrations
{
    /// <inheritdoc />
    public partial class add_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1bdb4c26-ae1d-45dd-a104-beafca292a7c", null, "admin", "admin" },
                    { "4fe32a9f-0bc7-4e25-9fa3-28c69d907fff", null, "client", "client" },
                    { "e14f6edd-8308-4a8f-9941-9f642b21c8c8", null, "king", "king" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bdb4c26-ae1d-45dd-a104-beafca292a7c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4fe32a9f-0bc7-4e25-9fa3-28c69d907fff");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e14f6edd-8308-4a8f-9941-9f642b21c8c8");
        }
    }
}
