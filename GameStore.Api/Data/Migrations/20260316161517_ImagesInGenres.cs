using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImagesInGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Genres",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ImageId",
                table: "Genres",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Images_ImageId",
                table: "Genres",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Images_ImageId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_ImageId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Genres");
        }
    }
}
