namespace TravelBook.Api.Models
{
    public class RefreshSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Користувач, якому належить сесія.
        public Guid UserId { get; set; }

        // Сам refresh token не зберігаємо.
        // У БД буде тільки його hash.
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAtUtc { get; set; }

        // Якщо значення є — сесію відкликано.
        public DateTime? RevokedAtUtc { get; set; }

        // Наприклад: "Taras-PC".
        public string? DeviceName { get; set; }

        // Зв'язок із Users.
        public ApiUser? User { get; set; }
    }
}