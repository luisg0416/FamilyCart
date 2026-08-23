using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInviteCodeToFamily : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteCode",
                table: "Families",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Families_InviteCode",
                table: "Families",
                column: "InviteCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Families_InviteCode",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "Families");
        }
    }
}
