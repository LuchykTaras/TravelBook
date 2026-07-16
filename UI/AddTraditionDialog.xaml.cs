using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class AddTraditionDialog : Window
    {
        private readonly TraditionService _traditionService;
        private string _selectedImagePath = string.Empty;
        private readonly Tradition? _editingTradition;

        // Конструктор для додавання нової традиції
        public AddTraditionDialog()
        {
            InitializeComponent();
            _traditionService = new TraditionService();
        }

        // Конструктор для редагування існуючої традиції
        public AddTraditionDialog(Tradition tradition) : this()
        {
            _editingTradition = tradition;

            TxtTitle.Text = tradition.Title;
            TxtCountry.Text = tradition.Country;
            TxtCityOrVillage.Text = tradition.CityOrVillage;
            TxtDescription.Text = tradition.Description;
            _selectedImagePath = tradition.ImagePath ?? string.Empty;

            // Підставляємо існуючу категорію у список, якщо вона там є
            foreach (ComboBoxItem item in CmbCategory.Items)
            {
                if (item.Content?.ToString() == tradition.Category)
                {
                    CmbCategory.SelectedItem = item;
                    break;
                }
            }

            // Показуємо прев'ю фото, якщо воно є
            if (!string.IsNullOrEmpty(tradition.ImagePath) && File.Exists(tradition.ImagePath))
            {
                ImgPreview.Source = new BitmapImage(new Uri(Path.GetFullPath(tradition.ImagePath)));
            }
        }

        // Обробник вибору зображення
        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Зображення (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Оберіть фотографію для традиції"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;

                // Показуємо прев'ю обраного фото
                ImgPreview.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Будь ласка, введіть назву традиції.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtCountry.Text))
            {
                MessageBox.Show("Будь ласка, вкажіть країну.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtDescription.Text))
            {
                MessageBox.Show("Будь ласка, додайте хоча б короткий опис.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Дозволяємо змінній приймати null (string?), щоб прибрати попередження CS8600
                string? savedImagePath = null;

                // Якщо користувач обрав фотографію (нову або ту саму, що вибрав діалогом), копіюємо її в папку додатка
                if (!string.IsNullOrEmpty(_selectedImagePath)
                    && File.Exists(_selectedImagePath)
                    && (_editingTradition == null || _selectedImagePath != _editingTradition.ImagePath))
                {
                    // Створюємо папку "Images" поруч із .exe файлом, якщо її ще немає
                    string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    // Генеруємо унікальне ім'я файлу, щоб уникнути однакових назв
                    string? extension = Path.GetExtension(_selectedImagePath);
                    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    savedImagePath = Path.Combine(imagesFolder, uniqueFileName);

                    // Копіюємо файл
                    File.Copy(_selectedImagePath, savedImagePath, true);

                    // Зберігаємо відносний шлях для зручності перенесення бази
                    savedImagePath = Path.Combine("Images", uniqueFileName);
                }

                // Безпечне приведення типу з використанням nullable string?
                string? selectedCategory = (CmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Загальна";

                var tradition = new Tradition
                {
                    Id = _editingTradition?.Id ?? 0,
                    Title = TxtTitle.Text.Trim(),
                    Country = TxtCountry.Text.Trim(),
                    CityOrVillage = string.IsNullOrWhiteSpace(TxtCityOrVillage.Text) ? null : TxtCityOrVillage.Text.Trim(),
                    Category = selectedCategory,
                    Description = TxtDescription.Text.Trim(),
                    // Якщо нове фото не обирали при редагуванні — лишаємо старий шлях
                    ImagePath = savedImagePath ?? _editingTradition?.ImagePath
                };

                if (_editingTradition != null)
                {
                    _traditionService.UpdateTradition(tradition);
                }
                else
                {
                    _traditionService.AddTradition(tradition);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося зберегти традицію: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}