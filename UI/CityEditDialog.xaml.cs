using Microsoft.Win32;
using System.Windows;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class CityEditDialog : Window
    {
        private readonly CityService _svc;
        private readonly City? _editing;

        public CityEditDialog(CityService svc)
        {
            InitializeComponent();
            _svc = svc;
        }

        public CityEditDialog(CityService svc, City city)
        {
            InitializeComponent();
            _svc = svc;
            _editing = city;
            TitleLabel.Text = "Редагувати місто";
            BtnSave.Content = "Оновити";
            TxtName.Text = city.Name;
            TxtCountry.Text = city.Country;
            TxtDescription.Text = city.Description;
            TxtImagePath.Text = city.ImagePath;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Зображення|*.jpg;*.jpeg;*.png;*.bmp|Всі файли|*.*"
            };
            if (dlg.ShowDialog() == true) TxtImagePath.Text = dlg.FileName;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) ||
                string.IsNullOrWhiteSpace(TxtCountry.Text))
            {
                MessageBox.Show("Назва та країна є обов'язковими полями.",
                    "Перевірка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_editing is null)
            {
                _svc.Add(new City
                {
                    Name = TxtName.Text.Trim(),
                    Country = TxtCountry.Text.Trim(),
                    Description = TxtDescription.Text.Trim(),
                    ImagePath = TxtImagePath.Text.Trim()
                });
            }
            else
            {
                _editing.Name = TxtName.Text.Trim();
                _editing.Country = TxtCountry.Text.Trim();
                _editing.Description = TxtDescription.Text.Trim();
                _editing.ImagePath = TxtImagePath.Text.Trim();
                _svc.Update(_editing);
            }
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}