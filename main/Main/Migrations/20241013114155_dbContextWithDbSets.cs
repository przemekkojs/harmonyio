using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class dbContextWithDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OpenDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizes_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Excersises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Question = table.Column<string>(type: "TEXT", nullable: false),
                    QuizId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excersises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Excersises_Quizes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizParticipants",
                columns: table => new
                {
                    ParticipantsId = table.Column<string>(type: "TEXT", nullable: false),
                    ParticipatedQuizesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizParticipants", x => new { x.ParticipantsId, x.ParticipatedQuizesId });
                    table.ForeignKey(
                        name: "FK_QuizParticipants_AspNetUsers_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizParticipants_Quizes_ParticipatedQuizesId",
                        column: x => x.ParticipatedQuizesId,
                        principalTable: "Quizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Grade = table.Column<int>(type: "INTEGER", nullable: false),
                    QuizId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizResults_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizResults_Quizes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExcersiseSolutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Answer = table.Column<string>(type: "TEXT", nullable: false),
                    ExcersiseId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ExcersiseResultId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcersiseSolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExcersiseSolutions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExcersiseSolutions_Excersises_ExcersiseId",
                        column: x => x.ExcersiseId,
                        principalTable: "Excersises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExcersiseResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    ExcersiseSolutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuizResultId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcersiseResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExcersiseResults_ExcersiseSolutions_ExcersiseSolutionId",
                        column: x => x.ExcersiseSolutionId,
                        principalTable: "ExcersiseSolutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExcersiseResults_QuizResults_QuizResultId",
                        column: x => x.QuizResultId,
                        principalTable: "QuizResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExcersiseResults_ExcersiseSolutionId",
                table: "ExcersiseResults",
                column: "ExcersiseSolutionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExcersiseResults_QuizResultId",
                table: "ExcersiseResults",
                column: "QuizResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Excersises_QuizId",
                table: "Excersises",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcersiseSolutions_ExcersiseId",
                table: "ExcersiseSolutions",
                column: "ExcersiseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcersiseSolutions_UserId",
                table: "ExcersiseSolutions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizes_CreatorId",
                table: "Quizes",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizParticipants_ParticipatedQuizesId",
                table: "QuizParticipants",
                column: "ParticipatedQuizesId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_QuizId",
                table: "QuizResults",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_UserId",
                table: "QuizResults",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcersiseResults");

            migrationBuilder.DropTable(
                name: "QuizParticipants");

            migrationBuilder.DropTable(
                name: "ExcersiseSolutions");

            migrationBuilder.DropTable(
                name: "QuizResults");

            migrationBuilder.DropTable(
                name: "Excersises");

            migrationBuilder.DropTable(
                name: "Quizes");
        }
    }
}
