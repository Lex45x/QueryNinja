using Microsoft.EntityFrameworkCore.Migrations;

namespace QueryNinja.Examples.AspNetCore.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Exams",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    Title = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Exams", x => x.Id); });

            migrationBuilder.CreateTable(
                "Students",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    Name = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Students", x => x.Id); });

            migrationBuilder.CreateTable(
                "Grades",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", value: true),
                    StudentId = table.Column<int>("INTEGER", nullable: false),
                    ExamId = table.Column<int>("INTEGER", nullable: false),
                    Mark = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        "FK_Grades_Exams_ExamId",
                        x => x.ExamId,
                        "Exams",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Grades_Students_StudentId",
                        x => x.StudentId,
                        "Students",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                "Exams",
                new[] {"Id", "Title"},
                new object[] {1, "Math"});

            migrationBuilder.InsertData(
                "Exams",
                new[] {"Id", "Title"},
                new object[] {2, "Physics"});

            migrationBuilder.InsertData(
                "Exams",
                new[] {"Id", "Title"},
                new object[] {3, "English"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {1, "Dina Davies"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {2, "Zayna Rivas"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {3, "Margo Alcock"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {4, "Ayden Ashton"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {5, "Heena Norton"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {6, "Mica Hills"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {7, "Roshni Rigby"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {8, "Quentin Farley"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {9, "Zubair Hurley"});

            migrationBuilder.InsertData(
                "Students",
                new[] {"Id", "Name"},
                new object[] {10, "Atlanta Lim"});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {1, 1, 4, 1});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {18, 1, 3, 10});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {17, 3, 4, 9});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {16, 2, 5, 9});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {15, 1, 3, 9});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {14, 3, 4, 8});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {13, 1, 1, 8});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {12, 3, 5, 7});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {11, 2, 5, 7});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {10, 1, 5, 7});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {9, 3, 4, 6});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {8, 3, 5, 5});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {7, 2, 2, 5});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {6, 3, 1, 4});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {5, 2, 3, 4});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {4, 2, 3, 3});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {3, 1, 5, 3});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {2, 2, 3, 1});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {19, 2, 4, 10});

            migrationBuilder.InsertData(
                "Grades",
                new[] {"Id", "ExamId", "Mark", "StudentId"},
                new object[] {20, 3, 2, 10});

            migrationBuilder.CreateIndex(
                "IX_Grades_ExamId",
                "Grades",
                "ExamId");

            migrationBuilder.CreateIndex(
                "IX_Grades_StudentId",
                "Grades",
                "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Grades");

            migrationBuilder.DropTable(
                "Exams");

            migrationBuilder.DropTable(
                "Students");
        }
    }
}