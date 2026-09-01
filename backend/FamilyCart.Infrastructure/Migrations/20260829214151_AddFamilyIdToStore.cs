using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilyIdToStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FamilyId",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_FamilyId",
                table: "Stores",
                column: "FamilyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Families_FamilyId",
                table: "Stores",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Families_FamilyId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_FamilyId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                table: "Stores");
        }
    }
}
