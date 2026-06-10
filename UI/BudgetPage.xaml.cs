using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TravelBook.Logic;
using TravelBook.Models;

namespace TravelBook.UI
{
    public partial class BudgetPage : Page
    {
        private readonly BudgetService _budgetService = new();

        public BudgetPage()
        {
            InitializeComponent();
            DpExpenseDate.SelectedDate = DateTime.Today;
            RefreshBudget();
        }

        private void RefreshBudget()
        {
            var budget = _budgetService.GetBudget();
            var spent = budget.Expenses.Sum(expense => expense.Amount);

            TxtTotalBudget.Text = budget.TotalAmount == 0
                ? string.Empty
                : budget.TotalAmount.ToString("0.##");

            TxtBudgetTotal.Text = FormatMoney(budget.TotalAmount);
            TxtSpent.Text = FormatMoney(spent);
            TxtRemaining.Text = FormatMoney(budget.Remaining);

            TxtRemaining.Foreground = budget.Remaining < 0
                ? (System.Windows.Media.Brush)FindResource("DangerBrush")
                : (System.Windows.Media.Brush)FindResource("PrimaryBrush");

            var filteredExpenses = ApplyExpenseFilters(budget.Expenses);

            GridExpenses.ItemsSource = null;
            GridExpenses.ItemsSource = filteredExpenses;

            EmptyExpensesState.Visibility = filteredExpenses.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            RefreshStats(spent);
        }

        private List<Expense> ApplyExpenseFilters(IEnumerable<Expense> expenses)
        {
            var result = expenses;

            if (!string.IsNullOrWhiteSpace(TxtExpenseSearch?.Text))
            {
                var query = TxtExpenseSearch.Text.Trim().ToLowerInvariant();

                result = result.Where(expense =>
                    expense.Title.ToLowerInvariant().Contains(query) ||
                    GetCategoryName(expense.Category).ToLowerInvariant().Contains(query));
            }

            if (CmbExpenseFilter?.SelectedItem is ComboBoxItem item)
            {
                var tag = item.Tag?.ToString();

                if (!string.IsNullOrWhiteSpace(tag) && tag != "All")
                {
                    var category = ParseCategory(tag);
                    result = result.Where(expense => expense.Category == category);
                }
            }

            return result
                .OrderByDescending(expense => expense.Date)
                .ThenByDescending(expense => expense.Amount)
                .ToList();
        }

        private void RefreshStats(decimal spent)
        {
            var stats = _budgetService.GetExpensesByCategory()
                .OrderByDescending(pair => pair.Value)
                .Select(pair => new BudgetStatRow
                {
                    Category = GetCategoryName(pair.Key),
                    AmountText = FormatMoney(pair.Value),
                    Percent = spent == 0 ? 0 : (double)(pair.Value / spent * 100)
                })
                .ToList();

            StatsList.ItemsSource = stats;
            TxtNoStats.Visibility = stats.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnSetBudget_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(TxtTotalBudget.Text, out var total) || total < 0)
            {
                MessageBox.Show("Введіть коректну суму бюджету.", "Перевірка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _budgetService.SetTotalAmount(total);
            RefreshBudget();
        }

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtExpenseTitle.Text))
            {
                MessageBox.Show("Вкажіть назву витрати.", "Перевірка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtExpenseAmount.Text, out var amount) || amount <= 0)
            {
                MessageBox.Show("Введіть коректну суму витрати.", "Перевірка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _budgetService.AddExpense(new Expense
            {
                Title = TxtExpenseTitle.Text.Trim(),
                Category = ParseCategory((CmbExpenseCategory.SelectedItem as ComboBoxItem)?.Tag?.ToString()),
                Amount = amount,
                Date = DpExpenseDate.SelectedDate ?? DateTime.Today
            });

            TxtExpenseTitle.Clear();
            TxtExpenseAmount.Clear();
            DpExpenseDate.SelectedDate = DateTime.Today;
            RefreshBudget();
        }

        private void BtnRemoveExpense_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not Expense expense)
            {
                return;
            }

            var result = MessageBox.Show($"Видалити витрату «{expense.Title}»?", "Підтвердження",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var sourceExpense = _budgetService.GetBudget().Expenses.FirstOrDefault(item =>
                item.Title == expense.Title &&
                item.Category == expense.Category &&
                item.Amount == expense.Amount &&
                item.Date == expense.Date);

            if (sourceExpense is not null)
            {
                _budgetService.RemoveExpense(sourceExpense);
                RefreshBudget();
            }
        }

        private void ExpenseFilter_Changed(object sender, TextChangedEventArgs e)
        {
            if (GridExpenses is null)
            {
                return;
            }

            ExpenseSearchPlaceholder.Visibility = string.IsNullOrEmpty(TxtExpenseSearch.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            RefreshBudget();
        }

        private void ExpenseFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (GridExpenses is null)
            {
                return;
            }

            RefreshBudget();
        }

        private void TxtExpenseSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            ExpenseSearchPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TxtExpenseSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtExpenseSearch.Text))
            {
                ExpenseSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private static ExpenseCategory ParseCategory(string? tag)
        {
            return tag switch
            {
                "Housing" => ExpenseCategory.Housing,
                "Transport" => ExpenseCategory.Transport,
                "Food" => ExpenseCategory.Food,
                "Attractions" => ExpenseCategory.Attractions,
                "Other" => ExpenseCategory.Other,
                _ => ExpenseCategory.Housing
            };
        }

        private static string GetCategoryName(ExpenseCategory category)
        {
            return category switch
            {
                ExpenseCategory.Housing => "Житло",
                ExpenseCategory.Transport => "Транспорт",
                ExpenseCategory.Food => "Їжа",
                ExpenseCategory.Attractions => "Визначні місця",
                ExpenseCategory.Other => "Інше",
                _ => "Інше"
            };
        }

        private static string FormatMoney(decimal value)
        {
            return $"{value:N2} ₴";
        }

        private class BudgetStatRow
        {
            public string Category { get; set; } = string.Empty;
            public string AmountText { get; set; } = string.Empty;
            public double Percent { get; set; }
        }
    }
}
