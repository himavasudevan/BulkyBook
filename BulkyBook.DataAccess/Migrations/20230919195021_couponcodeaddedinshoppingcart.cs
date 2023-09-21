using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class couponcodeaddedinshoppingcart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<double>(
                name: "CouponAmount",
                table: "ShoppingCarts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "ShoppingCarts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_CouponId",
                table: "ShoppingCarts",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Coupons_CouponId",
                table: "ShoppingCarts",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Coupons_CouponId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_CouponId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "CouponAmount",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "ShoppingCarts");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
