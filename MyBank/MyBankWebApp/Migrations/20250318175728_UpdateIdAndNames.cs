using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBankWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AccountDetails",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "Transactions",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
               name: "Reciver",
               table: "Transactions",
               newName: "ReceiverId");

            migrationBuilder.RenameTable(
                name: "AccountDetails",
                newName: "Accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AccountDetails",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Transactions",
                newName: "Sender");

            migrationBuilder.RenameColumn(
               name: "ReceiverId",
               table: "Transactions",
               newName: "Reciver");

            migrationBuilder.RenameTable(
               newName: "Accounts",
               name: "AccountDetails");
        }
    }
}
