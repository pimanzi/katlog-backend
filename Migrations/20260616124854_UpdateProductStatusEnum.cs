using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace katlog_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Status",
                table: "Products");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Status",
                table: "Products",
                sql: "\"Status\" IN ('Draft','InReview','ReadyToPublish', 'Published','Archived')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Status",
                table: "Products");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Status",
                table: "Products",
                sql: "\"Status\" IN ('Draft','Review','Published','Archived')");
        }
    }
}
