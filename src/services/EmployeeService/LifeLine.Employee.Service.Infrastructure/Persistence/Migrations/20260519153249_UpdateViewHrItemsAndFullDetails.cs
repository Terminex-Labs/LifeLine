using LifeLine.Employee.Service.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeLine.Employee.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateViewHrItemsAndFullDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER VIEW ""V_Employee_Hr_Items"" 
                RENAME COLUMN ""ImageKey"" TO ""PersonalPhoto"";
            ");
            migrationBuilder.CreateView("V_Employee_Hr_Items");

            migrationBuilder.Sql(@"
                ALTER VIEW ""V_Employee_Full_Details"" 
                RENAME COLUMN ""Avatar"" TO ""PersonalPhoto"";
            ");
            migrationBuilder.CreateView("V_Employee_Full_Details");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropView("V_Employee_Hr_Items");
            migrationBuilder.DropView("V_Employee_Full_Details");
        }
    }
}
