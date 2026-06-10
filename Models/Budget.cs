namespace TravelBook.Models
{
    public class Budget
    {
        public decimal TotalAmount { get; set; }
        public List<Expense> Expenses { get; set; } = new();
        public decimal Remaining => TotalAmount - Expenses.Sum(expense => expense.Amount);
    }
}
