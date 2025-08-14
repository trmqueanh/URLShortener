using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URLShortener.MVC.Migrations
{
    /// <inheritdoc />
    public partial class RenameTableToCustomizedURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomtedURL",
                table: "CustomtedURL");

            migrationBuilder.RenameTable(
                name: "CustomtedURL",
                newName: "CustomizedURL");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomizedURL",
                table: "CustomizedURL",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomizedURL",
                table: "CustomizedURL");

            migrationBuilder.RenameTable(
                name: "CustomizedURL",
                newName: "CustomtedURL");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomtedURL",
                table: "CustomtedURL",
                column: "Id");
        }
    }
}
