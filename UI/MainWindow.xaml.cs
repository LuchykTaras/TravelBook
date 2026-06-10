using System.Windows;

namespace TravelBook.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new CitiesPage());
        }

        private void BtnCities_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new CitiesPage());

        private void BtnRoutes_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new RoutePage());

        private void BtnBudget_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new BudgetPage());
    }
}
