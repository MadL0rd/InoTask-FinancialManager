using FinancialManagerWPF.Models;
using FinancialManagerWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class MainWindow : Window
    {
        ExpenseContext db;
        ExpenseViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            db = new ExpenseContext();
            try
            {
                db.LoadAll();
                viewModel = new ExpenseViewModel(db);
                DataContext = viewModel;
            }
            catch
            {
                ConnectionProblemGrid.Visibility = Visibility.Visible;
            }

            this.Closing += MainWindow_Closing;

            CategoryColor.Fill = GetRandomColor();
            CurrencyColor.Fill = GetRandomColor();

        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
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
        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            Category itemBuffer = (sender as Button).DataContext as Category;
            if (itemBuffer != null)
            {
                db.categories.Remove(itemBuffer);
                db.SaveChanges();
            }
        }
        private void DeleteExpense(object sender, RoutedEventArgs e)
        {
            Expense itemBuffer = (sender as Button).DataContext as Expense;
            if (itemBuffer != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту транзакцию?","Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (itemBuffer.currency != null)
                    {
                        itemBuffer.currency.balance -= itemBuffer.value;
                    }                    
                    db.expenses.Remove(itemBuffer);
                    db.SaveChanges();
                    RefreshAllMainMenu();
                }                
            }
        }



        private void AddCurrency(object sender, RoutedEventArgs e)
        {
            if (CurrencyNameBox.Text.Length == 0 || CurrencyBallanceBox.Text.Length == 0)
            {
                MessageBox.Show("Заполние необходимые поля!");
                return;
            }
            Currency buffCurrency = new Currency();
            buffCurrency.balance = Convert.ToDouble(CurrencyBallanceBox.Text);
            buffCurrency.title = CurrencyNameBox.Text;
            buffCurrency.color = CurrencyColor.Fill.ToString();

            db.currencies.Add(buffCurrency);
            db.SaveChanges();

            CurrencyBallanceBox.Text = "";
            CurrencyNameBox.Text = "";
            CurrencyColor.Fill = GetRandomColor();
        }
        private void AddCategory(object sender, RoutedEventArgs e)
        {
            if (CategoryNameBox.Text.Length == 0)
            {
                MessageBox.Show("Заполние необходимые поля!");
                return;
            }
            Category buffCategory = new Category();

            buffCategory.title = CategoryNameBox.Text;
            buffCategory.color = CategoryColor.Fill.ToString();

            db.categories.Add(buffCategory);
            db.SaveChanges();

            CategoryNameBox.Text = "";
            CategoryColor.Fill = GetRandomColor();
        }
        private void AddExpense(object sender, RoutedEventArgs e)
        {
            if (TransacrionCategoryComboBox.SelectedItem == null ||
                TransacrionCurrencyComboBox.SelectedItem == null ||
                TransacrionValueBox.Text.Length == 0 ||
                TransacrionTitleBox.Text.Length == 0
               )
            {
                MessageBox.Show("Заполние необходимые поля!");
                return;
            }
            if (TransactionDate.SelectedDate == null)
            {
                TransactionDate.SelectedDate = DateTime.Today;
            }

            Expense expenseBuff = new Expense();
            expenseBuff.category = TransacrionCategoryComboBox.SelectedItem as Category;
            expenseBuff.currency = TransacrionCurrencyComboBox.SelectedItem as Currency;
            expenseBuff.title = TransacrionTitleBox.Text;
            expenseBuff.value = Convert.ToDouble(TransacrionValueBox.Text);
            expenseBuff.date = TransactionDate.SelectedDate.Value;

            if (TransactionAdd.IsChecked == true)
            {
                expenseBuff.value *= -1;
            }
            expenseBuff.currency.balance -= expenseBuff.value;

            expenseBuff.value *= -1;
            db.expenses.Add(expenseBuff);
            db.SaveChanges();
            RefreshAllMainMenu();
        }



        private void ChooseColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Ellipse palette = sender as Ellipse;
                if (palette != null)
                {
                    SolidColorBrush CurrencyColor = new SolidColorBrush(Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B));
                    palette.Fill = CurrencyColor;

                    Group itemBuffer = palette.DataContext as Group;
                    if (itemBuffer != null)
                    {
                        itemBuffer.color = CurrencyColor.ToString();
                        db.SaveChanges();

                        RefreshAllMainMenu();
                    }
                }
            }
        }
        private void GetNewTitle(object sender, MouseButtonEventArgs e)
        {
            GetNewTitleWindow getNewTitle = new GetNewTitleWindow();

            if (getNewTitle.ShowDialog() == true)
            {
                Label label = sender as Label;
                if (label != null) 
                {
                    BaseData item = label.DataContext as BaseData;
                    item.title = getNewTitle.TitleFromUser;
                    label.Content = getNewTitle.TitleFromUser;

                    db.SaveChanges();

                    RefreshAllMainMenu();
                }
            }
        }
        private void EditExpense(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Expense item = button.DataContext as Expense;
                if (item != null)
                {
                    ExpenseEditWindow editWindow = new ExpenseEditWindow(viewModel, item);
                    if (editWindow.ShowDialog() == true)
                    {
                        db.SaveChanges();
                        RefreshAllMainMenu();
                    }                 
                }
            }
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



        private void RefreshAllMainMenu()
        {
            MainMeueCurrencyList.Items.Refresh();
            TransacrionCategoryComboBox.Items.Refresh();
            TransacrionCurrencyComboBox.Items.Refresh();
            CategoryFilterComboBox.Items.Refresh();
            CategoryFilterComboBox.Items.Refresh();
            ExpenseList.Items.Refresh();
        }
        public SolidColorBrush GetRandomColor()
        {
            Random random = new Random();
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(
                                        (byte)random.Next(1, 255),
                                        (byte)random.Next(1, 255),
                                        (byte)random.Next(1, 233))
                                      );
            return brush;
        }
        private void OnlyDigits(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
            {
                e.Handled = false;
            }
            else
            {
                if (c == ',')
                {
                    TextBox textBox = sender as TextBox;
                    if (textBox != null)
                    {
                        if (textBox.Text.IndexOf(',') == -1)
                        {
                            e.Handled = false;
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }


            base.OnPreviewTextInput(e);
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineLeft();
            else
                scrollviewer.LineRight();
            e.Handled = true;
        }

        private void ResetFilters(object sender, RoutedEventArgs e)
        {
            viewModel.ResetFilters();
        }
        private void CurrencyFilterReset(object sender, RoutedEventArgs e)
        {
            viewModel.CurrencyFilter = null;
        }
        private void CategoryFilterReset(object sender, RoutedEventArgs e)
        {
            viewModel.CategoryFilter = null;
        }
    }
}
