using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDA.Producer.Migrations
{
    /// <inheritdoc />
    public partial class addtraceIdforopentelemetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TraceId",
                table: "Outbox",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TraceId",
                table: "Outbox");
        }
    }
}
