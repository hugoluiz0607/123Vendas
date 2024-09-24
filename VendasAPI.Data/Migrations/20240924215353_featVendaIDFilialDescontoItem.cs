using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendasAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class featVendaIDFilialDescontoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FilialId",
                table: "Vendas",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Desconto",
                table: "ItensVenda",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilialId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "Desconto",
                table: "ItensVenda");
        }
    }
}
