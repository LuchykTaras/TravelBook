using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class CityDetailPage : Page
    {
        private readonly CityService _svc;
        private City _city;
        private AttractionCategory? _filterCategory;

        public CityDetailPage(City city, CityService svc)
        {
            InitializeComponent();
            _svc = svc;
            _city = city;
            BindCity();
            RefreshAttractions();
        }

        private void BindCity()
        {
            TxtPageTitle.Text = _city.Name;
            TxtCountry.Text = $"🌍  {_city.Country}";

            TxtDescription.Text = string.IsNullOrWhiteSpace(_city.Description)
                ? "(опис відсутній)"
                : _city.Description;

            if (!string.IsNullOrWhiteSpace(_city.ImagePath))
            {
                try
                {
                    ImgCity.Source = new BitmapImage(new Uri(_city.ImagePath));
                    ImgCity.Visibility = Visibility.Visible;
                }
                catch
                {
                    ImgCity.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ImgCity.Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshAttractions()
        {
            var items = _svc.GetAttractions(_city.Id, _filterCategory);

            if (!string.IsNullOrWhiteSpace(TxtAttractionSearch?.Text))
            {
                var query = TxtAttractionSearch.Text.Trim().ToLowerInvariant();

                items = items.Where(attraction =>
                    attraction.Name.ToLowerInvariant().Contains(query) ||
                    attraction.Address.ToLowerInvariant().Contains(query) ||
                    attraction.CategoryDisplayName.ToLowerInvariant().Contains(query));
            }

            var result = items
                .OrderByDescending(attraction => attraction.Rating)
                .ThenBy(attraction => attraction.Name)
                .ToList();

            GridAttractions.ItemsSource = null;
            GridAttractions.ItemsSource = result;

            EmptyAttractionsState.Visibility = result.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void BtnEditCity_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CityEditDialog(_svc, _city)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                _city = _svc.GetById(_city.Id)!;
                BindCity();
                RefreshAttractions();
            }
        }

        private void BtnDeleteCity_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Видалити місто «{_city.Name}»?", "Підтвердження",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _svc.Delete(_city.Id);
                NavigationService?.GoBack();
            }
        }

        private void BtnAddAttraction_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AttractionEditDialog(_svc, _city.Id)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                RefreshAttractions();
            }
        }

        private void BtnEditAttr_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not Attraction attraction)
            {
                return;
            }

            var dialog = new AttractionEditDialog(_svc, _city.Id, attraction)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                RefreshAttractions();
            }
        }

        private void BtnDeleteAttr_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not Attraction attraction)
            {
                return;
            }

            var result = MessageBox.Show($"Видалити «{attraction.Name}»?", "Підтвердження",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _svc.DeleteAttraction(_city.Id, attraction.Id);
                RefreshAttractions();
            }
        }

        private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_city is null || GridAttractions is null || CmbFilter.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            _filterCategory = item.Tag?.ToString() switch
            {
                "Museum" => AttractionCategory.Museum,
                "Park" => AttractionCategory.Park,
                "Restaurant" => AttractionCategory.Restaurant,
                "Hotel" => AttractionCategory.Hotel,
                "Other" => AttractionCategory.Other,
                _ => null
            };

            RefreshAttractions();
        }

        private void TxtAttractionSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            AttractionSearchPlaceholder.Visibility = string.IsNullOrEmpty(TxtAttractionSearch.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            RefreshAttractions();
        }

        private void TxtAttractionSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            AttractionSearchPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TxtAttractionSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtAttractionSearch.Text))
            {
                AttractionSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }
    }
}
