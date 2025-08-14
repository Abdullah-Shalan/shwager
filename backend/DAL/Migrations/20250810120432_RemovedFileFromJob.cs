using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFileFromJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Jobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
