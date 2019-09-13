using FinancialManagerWPF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinancialManagerWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ExpenseContext db;
        public MainWindow()
        {
            InitializeComponent();

            db = new ExpenseContext();
            db.categories.Load();
            db.currencies.Load();
            db.expenses.Load();
            CurrencyManager.ItemsSource = db.currencies.Local.ToBindingList();

            this.Closing += MainWindow_Closing;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
        }
        private void DeleteCurrency(object sender, RoutedEventArgs e)
        {
            Currency itemBuffer = (sender as Button).DataContext as Currency;
            if (itemBuffer != null)
            {
                db.currencies.Remove(itemBuffer);
                db.SaveChanges();
            }
        }
        private void ChooseColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Rectangle palette = sender as Rectangle;
                if (palette != null)
                {
                    SolidColorBrush CurrencyColor = new SolidColorBrush(Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B));
                    palette.Fill = CurrencyColor;

                    Currency itemBuffer = palette.DataContext as Currency;
                    if (itemBuffer != null)
                    {
                        itemBuffer.color = CurrencyColor.ToString();
                        db.SaveChanges();
                    }
                }
            }
        }

        private void AddCurrency(object sender, RoutedEventArgs e)
        {
            Currency buffCurrency = new Currency();
            buffCurrency.balance = 0;
            buffCurrency.title = CurrencyNameBox.Text;
            buffCurrency.color = CurrencyColor.Fill.ToString();

            db.currencies.Add(buffCurrency);
            db.SaveChanges();
        }

        private void ShowMainGrid(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Visible;
            CurrencyGrid.Visibility = Visibility.Hidden;
            CategoriesGrid.Visibility = Visibility.Hidden;
        }

        private void ShowCurrencyGrid(object sender, RoutedEventArgs e)
        {

            MainMenuGrid.Visibility = Visibility.Hidden;
            CurrencyGrid.Visibility = Visibility.Visible;
            CategoriesGrid.Visibility = Visibility.Hidden;
        }

        private void ShowCategoryGrid(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Hidden;
            CurrencyGrid.Visibility = Visibility.Hidden;
            CategoriesGrid.Visibility = Visibility.Visible;
        }
    }
}
