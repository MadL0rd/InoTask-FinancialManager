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
            try
            {
                db.categories.Load();
                db.currencies.Load();
                db.expenses.Load();

                CurrencyManager.ItemsSource = db.currencies.Local.ToBindingList();
                CategoryManager.ItemsSource = db.categories.Local.ToBindingList();

                this.Closing += MainWindow_Closing;
            }
            catch
            {
                ConnectionProblemGrid.Visibility = Visibility.Visible;
            }

            CategoryColor.Fill = GetRandomColor();
            CurrencyColor.Fill = GetRandomColor();
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

                    Group itemBuffer = palette.DataContext as Group;
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
            if (CurrencyNameBox.Text.Length == 0 || CurrencyBallanceBox.Text.Length == 0)
            {
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

        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            Category itemBuffer = (sender as Button).DataContext as Category;
            if (itemBuffer != null)
            {
                db.categories.Remove(itemBuffer);
                db.SaveChanges();
            }
        }

        private void AddCategory(object sender, RoutedEventArgs e)
        {
            if (CategoryNameBox.Text.Length == 0)
            {
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

        private void GetNewTitle(object sender, MouseButtonEventArgs e)
        {
            GetNewTitleWindow getNewTitle = new GetNewTitleWindow();

            if (getNewTitle.ShowDialog() == true)
            {
                Label label = sender as Label;
                if (label != null) 
                {
                    Group item = label.DataContext as Group;
                    item.title = getNewTitle.Title;
                    label.Content = getNewTitle.Title;

                    db.SaveChanges();
                }
            }
        }
    }
}
