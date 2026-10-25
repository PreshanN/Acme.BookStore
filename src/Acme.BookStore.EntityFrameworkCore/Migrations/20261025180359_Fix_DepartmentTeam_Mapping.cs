using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.BookStore.Migrations
{
    /// <inheritdoc />
    public partial class Fix_DepartmentTeam_Mapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDepartmentTeams_AppDepartmentTeams_DepartmentTeamConfigu~",
                table: "AppDepartmentTeams");

            migrationBuilder.DropTable(
                name: "DepartmentTeam");

            migrationBuilder.RenameColumn(
                name: "DepartmentTeamConfigurationId",
                table: "AppDepartmentTeams",
                newName: "DepartmentTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_AppDepartmentTeams_DepartmentTeamConfigurationId",
                table: "AppDepartmentTeams",
                newName: "IX_AppDepartmentTeams_DepartmentTeamId");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "AppDepartmentTeams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppDepartmentTeams_DepartmentId",
                table: "AppDepartmentTeams",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDepartmentTeams_AppDepartmentTeams_DepartmentTeamId",
                table: "AppDepartmentTeams",
                column: "DepartmentTeamId",
                principalTable: "AppDepartmentTeams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDepartmentTeams_AppDepartments_DepartmentId",
                table: "AppDepartmentTeams",
                column: "DepartmentId",
                principalTable: "AppDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDepartmentTeams_AppDepartmentTeams_DepartmentTeamId",
                table: "AppDepartmentTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDepartmentTeams_AppDepartments_DepartmentId",
                table: "AppDepartmentTeams");

            migrationBuilder.DropIndex(
                name: "IX_AppDepartmentTeams_DepartmentId",
                table: "AppDepartmentTeams");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AppDepartmentTeams");

            migrationBuilder.RenameColumn(
                name: "DepartmentTeamId",
                table: "AppDepartmentTeams",
                newName: "DepartmentTeamConfigurationId");

            migrationBuilder.RenameIndex(
                name: "IX_AppDepartmentTeams_DepartmentTeamId",
                table: "AppDepartmentTeams",
                newName: "IX_AppDepartmentTeams_DepartmentTeamConfigurationId");

            migrationBuilder.CreateTable(
                name: "DepartmentTeam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentTeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentTeam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentTeam_AppDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "AppDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentTeam_DepartmentTeam_DepartmentTeamId",
                        column: x => x.DepartmentTeamId,
                        principalTable: "DepartmentTeam",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTeam_DepartmentId",
                table: "DepartmentTeam",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTeam_DepartmentTeamId",
                table: "DepartmentTeam",
                column: "DepartmentTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDepartmentTeams_AppDepartmentTeams_DepartmentTeamConfigu~",
                table: "AppDepartmentTeams",
                column: "DepartmentTeamConfigurationId",
                principalTable: "AppDepartmentTeams",
                principalColumn: "Id");
        }
    }
}
