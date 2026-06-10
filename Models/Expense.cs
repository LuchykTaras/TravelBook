namespace TravelBook.Models
{
    public class Expense
    {
        public string Title { get; set; } = string.Empty;
        public ExpenseCategory Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public string CategoryDisplayName => Category switch
        {
            ExpenseCategory.Housing => "Житло",
            ExpenseCategory.Transport => "Транспорт",
            ExpenseCategory.Food => "Їжа",
            ExpenseCategory.Attractions => "Визначні місця",
            ExpenseCategory.Other => "Інше",
            _ => "Інше"
        };
    }
}
