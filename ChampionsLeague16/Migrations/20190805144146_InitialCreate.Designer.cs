﻿// <auto-generated />
using System;
using ChampionsLeague16.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChampionsLeague16.Migrations
{
    [DbContext(typeof(GameContext))]
    [Migration("20190805144146_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ChampionsLeague16.Data.Model.ScoreModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AwayTeam");

                    b.Property<string>("Group")
                        .HasMaxLength(1);

                    b.Property<string>("HomeTeam");

                    b.Property<DateTime>("KickOffDate");

                    b.Property<string>("LeagueTitle");

                    b.Property<int>("MatchDay");

                    b.Property<string>("Score");

                    b.HasKey("Id");

                    b.ToTable("Scores");
                });
#pragma warning restore 612, 618
        }
    }
}
