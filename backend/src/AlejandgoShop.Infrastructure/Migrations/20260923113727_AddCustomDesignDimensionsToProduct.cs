using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlejandgoShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomDesignDimensionsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomDesignDimensions",
                table: "Products",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomDesignDimensions",
                table: "Products");
        }
    }
}
