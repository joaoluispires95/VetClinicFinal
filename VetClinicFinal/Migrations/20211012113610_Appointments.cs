using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace VetClinic.Migrations
{
    public partial class Appointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                 name: "Employees",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "int", nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     Name = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                     Job = table.Column<string>(type: "nvarchar(MAX)", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Employees", x => x.Id);
                 });

            migrationBuilder.CreateTable(
                 name: "Appointments",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "int", nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     OwnerId = table.Column<int>(type: "int", nullable: false),
                     AnimalId = table.Column<int>(type: "int", nullable: false),
                     DoctorId = table.Column<int>(type: "int", nullable: false),
                     Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                     UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Appointments", x => x.Id);
                     table.ForeignKey(
                         name: "FK_Appointments_AspNetUsers_UserId",
                         column: x => x.UserId,
                         principalTable: "AspNetUsers",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Restrict);
                     table.ForeignKey(
                         name: "FK_Appointments_Animals_AnimalId",
                         column: x => x.AnimalId,
                         principalTable: "Animals",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Restrict);
                     table.ForeignKey(
                         name: "FK_Appointments_Clients_OwnerId",
                         column: x => x.OwnerId,
                         principalTable: "Clients",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Restrict);
                     table.ForeignKey(
                         name: "FK_Appointments_Employees_DoctorId",
                         column: x => x.DoctorId,
                         principalTable: "Employees",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Restrict);
                 });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
