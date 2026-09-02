using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using TravelBook.Models;

namespace TravelBook.DataTraditions
{
    public class TravelBookDbContext : DbContext
    {
        public DbSet<Tradition> Traditions { get; set; } = null!;

        public static string DatabasePath => GetDatabasePath();

        public TravelBookDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = GetDatabasePath();

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        private static string GetDatabasePath()
        {
            // Програма запускається з:
            // bin\Debug\net10.0-windows\
            //
            // Піднімаємося до каталогу, де знаходиться TravelBook.csproj.

            DirectoryInfo? directory =
                new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                string csprojPath =
                    Path.Combine(directory.FullName, "TravelBook.csproj");

                if (File.Exists(csprojPath))
                {
                    return Path.Combine(
                        directory.FullName,
                        "travelbook.db"
                    );
                }

                directory = directory.Parent;
            }

            // Резервний варіант.
            return Path.Combine(
                Environment.CurrentDirectory,
                "travelbook.db"
            );
        }
    }
}