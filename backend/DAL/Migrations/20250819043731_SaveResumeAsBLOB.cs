using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SaveResumeAsBLOB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResumeUrl",
                table: "Candidates",
                newName: "ResumeFileName");

            migrationBuilder.AddColumn<byte[]>(
                name: "ResumeData",
                table: "Candidates",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeData",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "ResumeFileName",
                table: "Candidates",
                newName: "ResumeUrl");
        }
    }
}
