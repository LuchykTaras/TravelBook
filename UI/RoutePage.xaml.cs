using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class RoutePage : Page
    {
        private readonly CityService _cityService = new();
        private readonly RouteService _routeService = new();
        private TravelRoute _currentRoute = new();

        public RoutePage()
        {
            InitializeComponent();
            RefreshCities();
            RefreshRoutes();
            StartNewRoute();
        }

        private void RefreshCities()
        {
            var cities = _cityService.GetAll()
                .OrderBy(city => city.Country)
                .ThenBy(city => city.Name)
                .ToList();

            CmbCities.ItemsSource = null;
            CmbCities.ItemsSource = cities;
            CmbCities.SelectedIndex = CmbCities.Items.Count > 0 ? 0 : -1;
        }

        private void RefreshRoutes()
        {
            var routes = _routeService.GetAll();

            if (!string.IsNullOrWhiteSpace(TxtRouteSearch?.Text))
            {
                var query = TxtRouteSearch.Text.Trim().ToLowerInvariant();

                routes = routes
                    .Where(route =>
                        route.Name.ToLowerInvariant().Contains(query) ||
                        route.Cities.Any(city =>
                            city.Name.ToLowerInvariant().Contains(query) ||
                            city.Country.ToLowerInvariant().Contains(query)))
                    .ToList();
            }

            var result = routes
                .OrderByDescending(route => route.CreatedDate)
                .ToList();

            LstRoutes.ItemsSource = null;
            LstRoutes.ItemsSource = result;
            TxtRouteCount.Text = $"{result.Count} маршрут(ів) знайдено";
        }

        private void StartNewRoute()
        {
            _currentRoute = new TravelRoute
            {
                Name = "Новий маршрут",
                CreatedDate = DateTime.Now,
                Cities = new List<City>()
            };

            TxtRouteName.Text = _currentRoute.Name;
            LstRoutes.SelectedItem = null;
            BindRouteCities();
        }

        private void BindRouteCities()
        {
            LstRouteCities.ItemsSource = null;
            LstRouteCities.ItemsSource = _currentRoute.Cities;

            EmptyRouteState.Visibility = _currentRoute.Cities.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            var status = _currentRoute.Id == 0
                ? "новий маршрут"
                : $"створено {_currentRoute.CreatedDate:dd.MM.yyyy HH:mm}";

            TxtSelectedInfo.Text = $"{status} · {_currentRoute.Cities.Count} міст(а) у маршруті";
        }

        private void BtnNewRoute_Click(object sender, RoutedEventArgs e)
        {
            StartNewRoute();
        }

        private void LstRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstRoutes.SelectedItem is not TravelRoute route)
            {
                return;
            }

            _currentRoute = route;
            TxtRouteName.Text = _currentRoute.Name;
            BindRouteCities();
        }

        private void BtnAddCity_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCities.SelectedItem is not City city)
            {
                MessageBox.Show("Спочатку додайте хоча б одне місто у модулі «Міста».",
                    "TravelBook", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _currentRoute.Cities.Add(city);
            BindRouteCities();
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var index = LstRouteCities.SelectedIndex;
            if (index <= 0)
            {
                return;
            }

            (_currentRoute.Cities[index - 1], _currentRoute.Cities[index]) =
                (_currentRoute.Cities[index], _currentRoute.Cities[index - 1]);

            BindRouteCities();
            LstRouteCities.SelectedIndex = index - 1;
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var index = LstRouteCities.SelectedIndex;
            if (index < 0 || index >= _currentRoute.Cities.Count - 1)
            {
                return;
            }

            (_currentRoute.Cities[index + 1], _currentRoute.Cities[index]) =
                (_currentRoute.Cities[index], _currentRoute.Cities[index + 1]);

            BindRouteCities();
            LstRouteCities.SelectedIndex = index + 1;
        }

        private void BtnRemoveCity_Click(object sender, RoutedEventArgs e)
        {
            if (LstRouteCities.SelectedItem is not City city)
            {
                return;
            }

            _currentRoute.Cities.Remove(city);
            BindRouteCities();
        }

        private void BtnSaveRoute_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtRouteName.Text))
            {
                MessageBox.Show("Вкажіть назву маршруту.", "Перевірка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentRoute.Name = TxtRouteName.Text.Trim();

            if (_currentRoute.Id == 0)
            {
                _routeService.Add(_currentRoute);
            }
            else
            {
                _routeService.Update(_currentRoute);
            }

            RefreshRoutes();
            LstRoutes.SelectedItem = _currentRoute;
            BindRouteCities();

            MessageBox.Show("Маршрут збережено у data/routes.json.", "TravelBook",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDeleteRoute_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRoute.Id == 0)
            {
                StartNewRoute();
                return;
            }

            var result = MessageBox.Show($"Видалити маршрут «{_currentRoute.Name}»?", "Підтвердження",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            _routeService.Delete(_currentRoute.Id);
            RefreshRoutes();
            StartNewRoute();
        }

        private void TxtRouteSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RouteSearchPlaceholder.Visibility = string.IsNullOrEmpty(TxtRouteSearch.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            RefreshRoutes();
        }

        private void TxtRouteSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            RouteSearchPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TxtRouteSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtRouteSearch.Text))
            {
                RouteSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }
    }
}
