using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    public class CinemaContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<MovieTags> MovieTags { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<SeatReservation> SeatReservations { get; set; }

        public CinemaContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// <summary>
            /// Initializing a multy-part primary key for MovieTag
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<MovieTags>()
                .HasKey(x => new { x.MovieId, x.TagId });

            /// <summary>
            /// Initializing a multy-part primary key for SeatReservation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<SeatReservation>()
                .HasKey(x => new { x.ReservationId, x.SeatId });

            /// <summary>
            /// Seat -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Seat>()
                .HasOne(x => x.Auditorium)
                .WithMany(x => x.Seats)
                .HasForeignKey(x => x.AuditoriumId)
                .IsRequired();

            /// <summary>
            /// Auditorium -> Seat relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()
                .HasMany(x => x.Seats)
                .WithOne(x => x.Auditorium)
                .IsRequired();


            /// <summary>
            /// Cinema -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Cinema>()
                .HasMany(x => x.Auditoriums)
                .WithOne(x => x.Cinema)
                .IsRequired();
            
            /// <summary>
            /// Auditorium -> Cinema relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()
                .HasOne(x => x.Cinema)
                .WithMany(x => x.Auditoriums)
                .HasForeignKey(x => x.CinemaId)
                .IsRequired();


            /// <summary>
            /// Auditorium -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()               
               .HasMany(x => x.Projections)
               .WithOne(x => x.Auditorium)
               .IsRequired();

            /// <summary>
            /// Projection -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasOne(x => x.Auditorium)
                .WithMany(x => x.Projections)
                .HasForeignKey(x => x.AuditoriumId)
                .IsRequired();


            /// <summary>
            /// Projection -> Movie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.Projections)
                .HasForeignKey(x => x.MovieId)
                .IsRequired();

            /// <summary>
            /// Movie -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Movie>()
                .HasMany(x => x.Projections)
                .WithOne(x => x.Movie)
                .IsRequired();

            /// <summary>
            /// Movie -> MovieTags relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Movie>()
                .HasMany(x => x.MovieTags)
                .WithOne(x => x.Movie)
                .IsRequired();

            /// <summary>
            /// MovieTags -> Movie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<MovieTags>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.MovieTags)
                .HasForeignKey(x => x.MovieId)
                .IsRequired();

            /// <summary>
            /// MovieTags -> Tags relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<MovieTags>()
                .HasOne(x => x.Tag)
                .WithMany(x => x.MovieTags)
                .HasForeignKey(x => x.TagId)
                .IsRequired();

            /// <summary>
            /// Tags -> MovieTags relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Tag>()
                .HasMany(x => x.MovieTags)
                .WithOne(x => x.Tag)
                .IsRequired();

            /// <summary>
            /// User -> Reservation relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<User>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.User)
                .IsRequired();

            /// <summary>
            /// Reservation -> User relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.User)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            /// <summary>
            /// Projection -> Reservation relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Projection)
                .IsRequired();

            /// <summary>
            /// Reservation -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.Projection)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.ProjectionId)
                .IsRequired();

            /// <summary>
            /// Reservation -> SeatReservation relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Reservation>()
                .HasMany(x => x.SeatReservation)
                .WithOne(x => x.Reservation)
                .IsRequired();

            /// <summary>
            /// SeatReservation -> Reservation relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<SeatReservation>()
                .HasOne(x => x.Reservation)
                .WithMany(x => x.SeatReservation)
                .HasForeignKey(x => x.ReservationId)
                .IsRequired();

            /// <summary>
            /// Seat -> SeatReservation relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Seat>()
                .HasMany(x => x.SeatReservations)
                .WithOne(x => x.Seat)
                .IsRequired();

            /// <summary>
            /// SeatReservation -> Seat relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<SeatReservation>()
                .HasOne(x => x.Seat)
                .WithMany(x => x.SeatReservations)
                .HasForeignKey(x => x.SeatId)
                .IsRequired();
        }
    }
}
