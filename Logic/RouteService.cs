using TravelBook.Models;

namespace TravelBook.Logic
{
    public class RouteService
    {
        private readonly List<TravelRoute> _routes;

        public RouteService()
        {
            _routes = DataService.LoadRoutes();
        }

        public IReadOnlyList<TravelRoute> GetAll() => _routes.AsReadOnly();

        public TravelRoute? GetById(int id) => _routes.FirstOrDefault(route => route.Id == id);

        public void Add(TravelRoute route)
        {
            route.Id = GetNextId();
            route.CreatedDate = route.CreatedDate == default ? DateTime.Now : route.CreatedDate;
            route.Cities ??= new List<City>();
            _routes.Add(route);
            Save();
        }

        public void Update(TravelRoute route)
        {
            var index = _routes.FindIndex(existingRoute => existingRoute.Id == route.Id);
            if (index < 0) return;

            route.Cities ??= new List<City>();
            _routes[index] = route;
            Save();
        }

        public void Delete(int id)
        {
            _routes.RemoveAll(route => route.Id == id);
            Save();
        }

        public void MoveCityUp(TravelRoute route, int cityIndex)
        {
            if (cityIndex <= 0 || cityIndex >= route.Cities.Count) return;
            (route.Cities[cityIndex - 1], route.Cities[cityIndex]) = (route.Cities[cityIndex], route.Cities[cityIndex - 1]);
            Update(route);
        }

        public void MoveCityDown(TravelRoute route, int cityIndex)
        {
            if (cityIndex < 0 || cityIndex >= route.Cities.Count - 1) return;
            (route.Cities[cityIndex + 1], route.Cities[cityIndex]) = (route.Cities[cityIndex], route.Cities[cityIndex + 1]);
            Update(route);
        }

        private int GetNextId() => _routes.Count == 0 ? 1 : _routes.Max(route => route.Id) + 1;
        private void Save() => DataService.SaveRoutes(_routes);
    }
}
