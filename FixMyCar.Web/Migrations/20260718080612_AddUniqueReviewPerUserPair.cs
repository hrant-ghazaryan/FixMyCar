using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixMyCar.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueReviewPerUserPair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId_TargetUserId",
                table: "Reviews",
                columns: new[] { "ReviewerId", "TargetUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewerId_TargetUserId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");
        }
    }
}
