﻿// <auto-generated />
using System;
using KinopoiskDB.Dal.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KinopoiskDB.Dal.PostgreSQL.Migrations
{
    [DbContext(typeof(KinopoiskDbContext))]
    partial class KinopoiskDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CountryMovie", b =>
                {
                    b.Property<int>("CountriesId")
                        .HasColumnType("integer")
                        .HasColumnName("countriesId");

                    b.Property<Guid>("MoviesId")
                        .HasColumnType("uuid")
                        .HasColumnName("moviesId");

                    b.HasKey("CountriesId", "MoviesId")
                        .HasName("pK_countryMovie");

                    b.HasIndex("MoviesId")
                        .HasDatabaseName("iX_countryMovie_moviesId");

                    b.ToTable("countryMovie", (string)null);
                });

            modelBuilder.Entity("GenreMovie", b =>
                {
                    b.Property<int>("GenresId")
                        .HasColumnType("integer")
                        .HasColumnName("genresId");

                    b.Property<Guid>("MoviesId")
                        .HasColumnType("uuid")
                        .HasColumnName("moviesId");

                    b.HasKey("GenresId", "MoviesId")
                        .HasName("pK_genreMovie");

                    b.HasIndex("MoviesId")
                        .HasDatabaseName("iX_genreMovie_moviesId");

                    b.ToTable("genreMovie", (string)null);
                });

            modelBuilder.Entity("KinopoiskDB.Core.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pK_countries");

                    b.ToTable("countries", (string)null);
                });

            modelBuilder.Entity("KinopoiskDB.Core.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pK_genres");

                    b.ToTable("genres", (string)null);
                });

            modelBuilder.Entity("KinopoiskDB.Core.Models.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<long>("KinopoiskId")
                        .HasColumnType("bigint")
                        .HasColumnName("kinopoiskId");

                    b.Property<string>("NameEn")
                        .HasColumnType("text")
                        .HasColumnName("nameEn");

                    b.Property<string>("NameOriginal")
                        .HasColumnType("text")
                        .HasColumnName("nameOriginal");

                    b.Property<string>("NameRu")
                        .HasColumnType("text")
                        .HasColumnName("nameRu");

                    b.Property<string>("PosterUrl")
                        .HasColumnType("text")
                        .HasColumnName("posterUrl");

                    b.Property<int?>("Year")
                        .HasColumnType("integer")
                        .HasColumnName("year");

                    b.HasKey("Id")
                        .HasName("pK_movies");

                    b.ToTable("movies", (string)null);
                });

            modelBuilder.Entity("CountryMovie", b =>
                {
                    b.HasOne("KinopoiskDB.Core.Models.Country", null)
                        .WithMany()
                        .HasForeignKey("CountriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fK_countryMovie_countries_countriesId");

                    b.HasOne("KinopoiskDB.Core.Models.Movie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fK_countryMovie_movies_moviesId");
                });

            modelBuilder.Entity("GenreMovie", b =>
                {
                    b.HasOne("KinopoiskDB.Core.Models.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fK_genreMovie_genres_genresId");

                    b.HasOne("KinopoiskDB.Core.Models.Movie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fK_genreMovie_movies_moviesId");
                });
#pragma warning restore 612, 618
        }
    }
}
