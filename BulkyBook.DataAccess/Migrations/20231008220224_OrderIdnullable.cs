using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OrderIdnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_OrderHeaders_OrderId",
                table: "WalletTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "WalletTransactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_OrderHeaders_OrderId",
                table: "WalletTransactions",
                column: "OrderId",
                principalTable: "OrderHeaders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_OrderHeaders_OrderId",
                table: "WalletTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_OrderHeaders_OrderId",
                table: "WalletTransactions",
                column: "OrderId",
                principalTable: "OrderHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
