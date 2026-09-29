using Microsoft.EntityFrameworkCore;
using TravelBook.Api.Models;

namespace TravelBook.Api.Data
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(
            DbContextOptions<ServerDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApiUser> Users => Set<ApiUser>();

        public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

        public DbSet<RefreshSession> RefreshSessions =>
            Set<RefreshSession>();

        public DbSet<Tradition> Traditions =>
            Set<Tradition>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // USERS
            // =========================
            modelBuilder.Entity<ApiUser>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });

            // =========================
            // OTP CODES
            // =========================
            modelBuilder.Entity<OtpCode>(entity =>
            {
                entity.ToTable("OtpCodes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.Property(x => x.CodeHash)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(x => x.Email);

                entity.HasIndex(x => x.ExpiresAtUtc);
            });

            // =========================
            // REFRESH SESSIONS
            // =========================
            modelBuilder.Entity<RefreshSession>(entity =>
            {
                entity.ToTable("RefreshSessions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.TokenHash)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(x => x.DeviceName)
                    .HasMaxLength(200);

                entity.HasIndex(x => x.TokenHash)
                    .IsUnique();

                entity.HasIndex(x => x.UserId);

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Sessions)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // TRADITIONS
            // =========================
            modelBuilder.Entity<Tradition>(entity =>
            {
                entity.ToTable("Traditions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(x => x.Country)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.CityOrVillage)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.ImageUrl)
                    .HasMaxLength(2000);

                entity.HasIndex(x => x.Country);

                entity.HasIndex(x => x.CityOrVillage);

                entity.HasIndex(x => x.Category);

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany(x => x.CreatedTraditions)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}