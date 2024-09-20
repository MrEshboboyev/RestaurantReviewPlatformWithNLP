using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSentimentScoreToReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "SentimentScore",
                table: "Reviews",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentimentScore",
                table: "Reviews");
        }
    }
}
