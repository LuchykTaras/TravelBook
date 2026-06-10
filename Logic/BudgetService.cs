using TravelBook.Models;

namespace TravelBook.Logic
{
    public class BudgetService
    {
        private Budget _budget;

        public BudgetService()
        {
            _budget = DataService.LoadBudget();
        }

        public Budget GetBudget() => _budget;

        public void SetTotalAmount(decimal totalAmount)
        {
            _budget.TotalAmount = Math.Max(0, totalAmount);
            Save();
        }

        public void AddExpense(Expense expense)
        {
            expense.Amount = Math.Max(0, expense.Amount);
            expense.Date = expense.Date == default ? DateTime.Now : expense.Date;
            _budget.Expenses.Add(expense);
            Save();
        }

        public void RemoveExpense(Expense expense)
        {
            _budget.Expenses.Remove(expense);
            Save();
        }

        public IReadOnlyDictionary<ExpenseCategory, decimal> GetExpensesByCategory()
        {
            return _budget.Expenses
                .GroupBy(expense => expense.Category)
                .ToDictionary(group => group.Key, group => group.Sum(expense => expense.Amount));
        }

        public void Reload() => _budget = DataService.LoadBudget();

        private void Save() => DataService.SaveBudget(_budget);
    }
}
