using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileModule.Migrations
{
    /// <inheritdoc />
    public partial class AddressFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Addresses_AddressId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_AddressId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "Addresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProfileId",
                table: "Addresses",
                column: "ProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Profiles_ProfileId",
                table: "Addresses",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Profiles_ProfileId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ProfileId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AddressId",
                table: "Profiles",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Addresses_AddressId",
                table: "Profiles",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
