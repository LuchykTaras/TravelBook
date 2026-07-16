namespace TravelBook.Models
{
    public class Tradition
    {
        public int Id { get; set; }

        // Назва традиції (наприклад: "Свято пампуха", "Маланка", "Водіння кози")
        public string Title { get; set; } = string.Empty;

        // Детальний опис звичаю чи обряду
        public string Description { get; set; } = string.Empty;

        // Країна (наприклад: "Україна")
        public string Country { get; set; } = string.Empty;

        // Назва міста або села (наприклад: "Пирогово", "Опішня")
        public string? CityOrVillage { get; set; }

        // Категорія звичаю (наприклад: "Свята", "Кулінарія", "Одяг", "Легенди")
        public string Category { get; set; } = "Загальна";

        public string? ImagePath { get; set; }
    }
}