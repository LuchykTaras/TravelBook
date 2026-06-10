using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class CitiesPage : Page
    {
        private readonly CityService _svc = new();

        public CitiesPage()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh(string? query = null)
        {
            var items = string.IsNullOrWhiteSpace(query)
                ? _svc.GetAll()
                : _svc.Search(query);

            var result = items
                .OrderBy(city => city.Country)
                .ThenBy(city => city.Name)
                .ToList();

            LstCities.ItemsSource = null;
            LstCities.ItemsSource = result;

            TxtCityCount.Text = string.IsNullOrWhiteSpace(query)
                ? $"{result.Count} місто(міст) у базі"
                : $"{result.Count} місто(міст) знайдено";

            EmptyState.Visibility = result.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(TxtSearch.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            Refresh(TxtSearch.Text);
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtSearch.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CityEditDialog(_svc)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                Refresh(TxtSearch.Text);
            }
        }

        private void LstCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstCities.SelectedItem is City city)
            {
                NavigationService?.Navigate(new CityDetailPage(city, _svc));
                LstCities.SelectedItem = null;
            }
        }
    }
}
