using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TravelBook.Models;

namespace TravelBook.Logic
{
    public static class DataService
    {
        private static readonly string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        private static readonly string CitiesFile = Path.Combine(DataDir, "cities.json");
        private static readonly string RoutesFile = Path.Combine(DataDir, "routes.json");
        private static readonly string BudgetFile = Path.Combine(DataDir, "budget.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        static DataService()
        {
            Directory.CreateDirectory(DataDir);
        }

        public static List<City> LoadCities() => LoadFromFile(CitiesFile, new List<City>());
        public static void SaveCities(List<City> cities) => SaveToFile(CitiesFile, cities);

        public static List<TravelRoute> LoadRoutes() => LoadFromFile(RoutesFile, new List<TravelRoute>());
        public static void SaveRoutes(List<TravelRoute> routes) => SaveToFile(RoutesFile, routes);

        public static Budget LoadBudget() => LoadFromFile(BudgetFile, new Budget());
        public static void SaveBudget(Budget budget) => SaveToFile(BudgetFile, budget);

        private static T LoadFromFile<T>(string path, T defaultValue)
        {
            if (!File.Exists(path))
            {
                SaveToFile(path, defaultValue);
                return defaultValue;
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return defaultValue;

            return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? defaultValue;
        }

        private static void SaveToFile<T>(string path, T data)
        {
            Directory.CreateDirectory(DataDir);
            var json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(path, json);
        }
    }
}
