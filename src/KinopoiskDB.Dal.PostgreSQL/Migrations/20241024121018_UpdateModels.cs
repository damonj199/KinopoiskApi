using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinopoiskDB.Dal.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountriesMovies");

            migrationBuilder.DropTable(
                name: "GenresMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                table: "Movies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genres",
                table: "Genres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.RenameTable(
                name: "Movies",
                newName: "movies");

            migrationBuilder.RenameTable(
                name: "Genres",
                newName: "genres");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "countries");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "movies",
                newName: "year");

            migrationBuilder.RenameColumn(
                name: "PosterUrl",
                table: "movies",
                newName: "posterUrl");

            migrationBuilder.RenameColumn(
                name: "NameRu",
                table: "movies",
                newName: "nameRu");

            migrationBuilder.RenameColumn(
                name: "NameOriginal",
                table: "movies",
                newName: "nameOriginal");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "movies",
                newName: "nameEn");

            migrationBuilder.RenameColumn(
                name: "KinopoiskId",
                table: "movies",
                newName: "kinopoiskId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "movies",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "movies",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "genres",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "genres",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "countries",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "countries",
                newName: "value");

            migrationBuilder.AddPrimaryKey(
                name: "pK_movies",
                table: "movies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pK_genres",
                table: "genres",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pK_countries",
                table: "countries",
                column: "id");

            migrationBuilder.CreateTable(
                name: "countryMovie",
                columns: table => new
                {
                    countriesId = table.Column<int>(type: "integer", nullable: false),
                    moviesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_countryMovie", x => new { x.countriesId, x.moviesId });
                    table.ForeignKey(
                        name: "fK_countryMovie_countries_countriesId",
                        column: x => x.countriesId,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fK_countryMovie_movies_moviesId",
                        column: x => x.moviesId,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "genreMovie",
                columns: table => new
                {
                    genresId = table.Column<int>(type: "integer", nullable: false),
                    moviesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_genreMovie", x => new { x.genresId, x.moviesId });
                    table.ForeignKey(
                        name: "fK_genreMovie_genres_genresId",
                        column: x => x.genresId,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fK_genreMovie_movies_moviesId",
                        column: x => x.moviesId,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "iX_countryMovie_moviesId",
                table: "countryMovie",
                column: "moviesId");

            migrationBuilder.CreateIndex(
                name: "iX_genreMovie_moviesId",
                table: "genreMovie",
                column: "moviesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "countryMovie");

            migrationBuilder.DropTable(
                name: "genreMovie");

            migrationBuilder.DropPrimaryKey(
                name: "pK_movies",
                table: "movies");

            migrationBuilder.DropPrimaryKey(
                name: "pK_genres",
                table: "genres");

            migrationBuilder.DropPrimaryKey(
                name: "pK_countries",
                table: "countries");

            migrationBuilder.RenameTable(
                name: "movies",
                newName: "Movies");

            migrationBuilder.RenameTable(
                name: "genres",
                newName: "Genres");

            migrationBuilder.RenameTable(
                name: "countries",
                newName: "Countries");

            migrationBuilder.RenameColumn(
                name: "year",
                table: "Movies",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "posterUrl",
                table: "Movies",
                newName: "PosterUrl");

            migrationBuilder.RenameColumn(
                name: "nameRu",
                table: "Movies",
                newName: "NameRu");

            migrationBuilder.RenameColumn(
                name: "nameOriginal",
                table: "Movies",
                newName: "NameOriginal");

            migrationBuilder.RenameColumn(
                name: "nameEn",
                table: "Movies",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "kinopoiskId",
                table: "Movies",
                newName: "KinopoiskId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Movies",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Movies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Genres",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Genres",
                newName: "Genre");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Countries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Countries",
                newName: "Country");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                table: "Movies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genres",
                table: "Genres",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CountriesMovies",
                columns: table => new
                {
                    CountriesId = table.Column<int>(type: "integer", nullable: false),
                    MoviesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountriesMovies", x => new { x.CountriesId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_CountriesMovies_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountriesMovies_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenresMovies",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "integer", nullable: false),
                    MoviesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenresMovies", x => new { x.GenresId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_GenresMovies_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenresMovies_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountriesMovies_MoviesId",
                table: "CountriesMovies",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_GenresMovies_MoviesId",
                table: "GenresMovies",
                column: "MoviesId");
        }
    }
}
