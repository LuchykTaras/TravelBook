using TravelBook.Models;

namespace TravelBook.Logic
{
    public class CityService
    {
        private readonly List<City> _cities;

        public CityService()
        {
            _cities = DataService.LoadCities();
        }

        public IReadOnlyList<City> GetAll() => _cities.AsReadOnly();

        public IEnumerable<City> Search(string? query)
        {
            if (string.IsNullOrWhiteSpace(query)) return _cities;

            var normalizedQuery = query.Trim().ToLowerInvariant();
            return _cities.Where(city =>
                city.Name.ToLowerInvariant().Contains(normalizedQuery) ||
                city.Country.ToLowerInvariant().Contains(normalizedQuery));
        }

        public City? GetById(int id) => _cities.FirstOrDefault(city => city.Id == id);

        public void Add(City city)
        {
            city.Id = GetNextCityId();
            city.Attractions ??= new List<Attraction>();
            _cities.Add(city);
            Save();
        }

        public void Update(City city)
        {
            var index = _cities.FindIndex(existingCity => existingCity.Id == city.Id);
            if (index < 0) return;

            city.Attractions ??= new List<Attraction>();
            _cities[index] = city;
            Save();
        }

        public void Delete(int id)
        {
            _cities.RemoveAll(city => city.Id == id);
            Save();
        }

        public IEnumerable<Attraction> GetAttractions(int cityId, AttractionCategory? filter = null)
        {
            var city = GetById(cityId);
            if (city is null) return Enumerable.Empty<Attraction>();

            return filter.HasValue
                ? city.Attractions.Where(attraction => attraction.Category == filter.Value)
                : city.Attractions;
        }

        public void AddAttraction(int cityId, Attraction attraction)
        {
            var city = GetRequiredCity(cityId);
            attraction.Id = GetNextAttractionId(city);
            attraction.Rating = Math.Clamp(attraction.Rating, 1, 5);
            city.Attractions.Add(attraction);
            Save();
        }

        public void UpdateAttraction(int cityId, Attraction attraction)
        {
            var city = GetRequiredCity(cityId);
            var index = city.Attractions.FindIndex(existingAttraction => existingAttraction.Id == attraction.Id);
            if (index < 0) return;

            attraction.Rating = Math.Clamp(attraction.Rating, 1, 5);
            city.Attractions[index] = attraction;
            Save();
        }

        public void DeleteAttraction(int cityId, int attractionId)
        {
            var city = GetById(cityId);
            if (city is null) return;

            city.Attractions.RemoveAll(attraction => attraction.Id == attractionId);
            Save();
        }

        private City GetRequiredCity(int cityId)
            => GetById(cityId) ?? throw new InvalidOperationException("Місто не знайдено.");

        private int GetNextCityId()
            => _cities.Count == 0 ? 1 : _cities.Max(city => city.Id) + 1;

        private static int GetNextAttractionId(City city)
            => city.Attractions.Count == 0 ? 1 : city.Attractions.Max(attraction => attraction.Id) + 1;

        private void Save() => DataService.SaveCities(_cities);
    }
}
