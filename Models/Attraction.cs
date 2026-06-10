namespace TravelBook.Models
{
    public class Attraction
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public AttractionCategory Category { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal EntryPrice { get; set; }
        public int Rating { get; set; } = 1;

        public string CategoryDisplayName => Category switch
        {
            AttractionCategory.Museum => "Музей",
            AttractionCategory.Park => "Парк",
            AttractionCategory.Restaurant => "Ресторан",
            AttractionCategory.Hotel => "Готель",
            AttractionCategory.Other => "Інше",
            _ => "Інше"
        };

        public string RatingStars
        {
            get
            {
                var normalizedRating = Math.Clamp(Rating, 1, 5);
                return new string('★', normalizedRating) + new string('☆', 5 - normalizedRating);
            }
        }
    }
}
