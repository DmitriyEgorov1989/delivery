using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "courier",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    speed = table.Column<int>(type: "integer", nullable: false),
                    coordinate_x = table.Column<int>(type: "integer", nullable: false),
                    coordinate_y = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courier", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    coordinate_x = table.Column<int>(type: "integer", nullable: true),
                    coordinate_y = table.Column<int>(type: "integer", nullable: true),
                    volume = table.Column<int>(type: "integer", nullable: false),
                    order_status = table.Column<string>(type: "text", nullable: true),
                    courier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    storage_place_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "storagePlaces",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    total_volume = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    courier_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storagePlaces", x => x.id);
                    table.ForeignKey(
                        name: "FK_storagePlaces_courier_courier_id",
                        column: x => x.courier_id,
                        principalTable: "courier",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_storagePlaces_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_storagePlaces_courier_id",
                table: "storagePlaces",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "IX_storagePlaces_order_id",
                table: "storagePlaces",
                column: "order_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "storagePlaces");

            migrationBuilder.DropTable(
                name: "courier");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
