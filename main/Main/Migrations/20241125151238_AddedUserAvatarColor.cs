using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserAvatarColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvatarColorId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarColorId",
                table: "AspNetUsers");
        }
    }
}
