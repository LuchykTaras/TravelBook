namespace TravelBook.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public List<Attraction> Attractions { get; set; } = new();

        public override string ToString() => $"{Name}, {Country}";
    }
}
