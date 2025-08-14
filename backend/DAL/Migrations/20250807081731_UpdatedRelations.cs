using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Jobs_JobId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_candidateTasks_Candidates_CandidateId",
                table: "candidateTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_candidateTasks_JobTasks_JobTaskId",
                table: "candidateTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_candidateTasks",
                table: "candidateTasks");

            migrationBuilder.RenameTable(
                name: "candidateTasks",
                newName: "CandidateTasks");

            migrationBuilder.RenameIndex(
                name: "IX_candidateTasks_JobTaskId",
                table: "CandidateTasks",
                newName: "IX_CandidateTasks_JobTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_candidateTasks_CandidateId",
                table: "CandidateTasks",
                newName: "IX_CandidateTasks_CandidateId");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Hrs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "Candidates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumeUrl",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateTasks",
                table: "CandidateTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Jobs_JobId",
                table: "Candidates",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateTasks_Candidates_CandidateId",
                table: "CandidateTasks",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateTasks_JobTasks_JobTaskId",
                table: "CandidateTasks",
                column: "JobTaskId",
                principalTable: "JobTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Jobs_JobId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateTasks_Candidates_CandidateId",
                table: "CandidateTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateTasks_JobTasks_JobTaskId",
                table: "CandidateTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateTasks",
                table: "CandidateTasks");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Hrs");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ResumeUrl",
                table: "Candidates");

            migrationBuilder.RenameTable(
                name: "CandidateTasks",
                newName: "candidateTasks");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateTasks_JobTaskId",
                table: "candidateTasks",
                newName: "IX_candidateTasks_JobTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateTasks_CandidateId",
                table: "candidateTasks",
                newName: "IX_candidateTasks_CandidateId");

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "Candidates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_candidateTasks",
                table: "candidateTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Jobs_JobId",
                table: "Candidates",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_candidateTasks_Candidates_CandidateId",
                table: "candidateTasks",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_candidateTasks_JobTasks_JobTaskId",
                table: "candidateTasks",
                column: "JobTaskId",
                principalTable: "JobTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
