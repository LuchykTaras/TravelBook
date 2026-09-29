namespace TravelBook.Api.Models
{
    public class Tradition
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string CityOrVillage { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        // На сервері використовуватимемо URL,
        // а не локальний шлях C:\Users\...
        public string? ImageUrl { get; set; }

        // Хто створив традицію.
        public Guid CreatedByUserId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Зв'язок із Users.
        public ApiUser? CreatedByUser { get; set; }
    }
}