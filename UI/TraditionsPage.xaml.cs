using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class TraditionsPage : Page
    {
        private readonly TraditionService _traditionService;
        private List<Tradition> _allTraditions;

        public TraditionsPage()
        {
            InitializeComponent();
            _traditionService = new TraditionService();
            _allTraditions = new List<Tradition>();
        }

        // Викликається при завантаженні сторінки
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        // Оновлення списку з бази даних
        private void RefreshData()
        {
            try
            {
                _allTraditions = _traditionService.GetAllTraditions();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження традицій: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            string query = TxtSearch.Text.Trim().ToLower();
            List<Tradition> filtered;

            if (string.IsNullOrWhiteSpace(query))
            {
                filtered = _allTraditions;
            }
            else
            {
                filtered = _allTraditions.Where(t =>
                    t.Title.ToLower().Contains(query) ||
                    t.Country.ToLower().Contains(query) ||
                    (t.CityOrVillage != null && t.CityOrVillage.ToLower().Contains(query)) ||
                    t.Description.ToLower().Contains(query)
                ).ToList();
            }

            LstTraditions.ItemsSource = filtered;
            TxtTraditionsCount.Text = $"{filtered.Count} традицій у базі";

            // Показуємо заглушку, якщо порожньо
            EmptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Пошук в реальному часі
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        // Видалення традиції
        private void BtnDeleteTradition_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var result = MessageBox.Show("Ви впевнені, що хочете видалити цю традицію?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _traditionService.DeleteTradition(id);
                    RefreshData();
                }
            }
        }

        // Редагування традиції
        private void BtnEditTradition_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tradition tradition)
            {
                var dialog = new AddTraditionDialog(tradition);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true)
                {
                    RefreshData();
                }
            }
        }

        // Поведінка плейсхолдера пошуку
        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        // Кнопка відкриття вікна додавання нової традиції (ОСЬ ТУТ МИ ВИПРАВИВ ПОМИЛКУ!)
        private void BtnAddTradition_Click(object sender, RoutedEventArgs e)
        {
            // Створюємо та показуємо наше гарне вікно замість старого повідомлення
            var dialog = new AddTraditionDialog();
            dialog.Owner = Window.GetWindow(this); // Щоб воно відкрилося гарно по центру програми

            // Якщо користувач успішно зберіг традицію, оновлюємо список на екрані
            if (dialog.ShowDialog() == true)
            {
                RefreshData();
            }
        }
    }
}