using System.Windows;
using System.Windows.Controls;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class AttractionEditDialog : Window
    {
        private readonly CityService _svc;
        private readonly int _cityId;
        private readonly Attraction? _editing;

        public AttractionEditDialog(CityService svc, int cityId)
        {
            InitializeComponent();
            _svc = svc;
            _cityId = cityId;
        }

        public AttractionEditDialog(CityService svc, int cityId, Attraction attraction)
        {
            InitializeComponent();
            _svc = svc;
            _cityId = cityId;
            _editing = attraction;
            TitleLabel.Text = "Редагувати пам'ятку";
            BtnSave.Content = "Оновити";
            TxtName.Text = attraction.Name;
            TxtAddress.Text = attraction.Address;
            TxtPrice.Text = attraction.EntryPrice.ToString();

            foreach (ComboBoxItem item in CmbCategory.Items)
                if (item.Tag?.ToString() == attraction.Category.ToString())
                { item.IsSelected = true; break; }

            foreach (ComboBoxItem item in CmbRating.Items)
                if (item.Tag?.ToString() == attraction.Rating.ToString())
                { item.IsSelected = true; break; }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Назва є обов'язковою.", "Перевірка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal.TryParse(TxtPrice.Text, out decimal price);
            int.TryParse((CmbRating.SelectedItem as ComboBoxItem)?.Tag?.ToString(), out int rating);
            if (rating == 0) rating = 3;

            var category = (CmbCategory.SelectedItem as ComboBoxItem)?.Tag?.ToString() switch
            {
                "Park" => AttractionCategory.Park,
                "Restaurant" => AttractionCategory.Restaurant,
                "Hotel" => AttractionCategory.Hotel,
                "Other" => AttractionCategory.Other,
                _ => AttractionCategory.Museum
            };

            if (_editing is null)
            {
                _svc.AddAttraction(_cityId, new Attraction
                {
                    Name = TxtName.Text.Trim(),
                    Category = category,
                    Address = TxtAddress.Text.Trim(),
                    EntryPrice = price,
                    Rating = rating
                });
            }
            else
            {
                _editing.Name = TxtName.Text.Trim(); _editing.Category = category;
                _editing.Address = TxtAddress.Text.Trim(); _editing.EntryPrice = price;
                _editing.Rating = rating;
                _svc.UpdateAttraction(_cityId, _editing);
            }
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}