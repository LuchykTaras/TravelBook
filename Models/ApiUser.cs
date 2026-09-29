namespace TravelBook.Api.Models
{
    public class ApiUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAtUtc { get; set; }

        public List<RefreshSession> Sessions { get; set; } = new();

        public List<Tradition> CreatedTraditions { get; set; } = new();
    }
}