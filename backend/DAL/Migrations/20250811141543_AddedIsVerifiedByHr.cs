using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsVerifiedByHr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedByHr",
                table: "CandidateTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerifiedByHr",
                table: "CandidateTasks");
        }
    }
}
