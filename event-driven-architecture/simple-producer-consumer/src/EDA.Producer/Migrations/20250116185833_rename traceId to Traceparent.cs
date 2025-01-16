using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDA.Producer.Migrations
{
    /// <inheritdoc />
    public partial class renametraceIdtoTraceparent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TraceId",
                table: "Outbox",
                newName: "TraceParent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TraceParent",
                table: "Outbox",
                newName: "TraceId");
        }
    }
}
