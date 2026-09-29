namespace TravelBook.Api.Models
{
    public class OtpCode
    {
        public long Id { get; set; }

        public string Email { get; set; } = string.Empty;

        // Сам 6-значний код у БД не зберігаємо.
        // Тут буде тільки його hash.
        public string CodeHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAtUtc { get; set; }

        // Якщо значення є — код уже використали.
        public DateTime? UsedAtUtc { get; set; }

        // Кількість неправильних спроб введення.
        public int FailedAttempts { get; set; }
    }
}