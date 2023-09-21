using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OrderCouponNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CouponAmount",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "OrderHeaders",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_CouponId",
                table: "OrderHeaders",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_CouponId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CouponAmount",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "OrderHeaders");
        }
    }
}
