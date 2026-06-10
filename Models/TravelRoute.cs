namespace TravelBook.Models
{
    public class TravelRoute
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<City> Cities { get; set; } = new();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
