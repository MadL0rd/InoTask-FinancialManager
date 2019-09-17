using FinancialManagerWPF.Models;
using FinancialManagerWPF.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FinancialManagerWPF
{
    /// <summary>
    /// Логика взаимодействия для ExpenseEditWindow.xaml
    /// </summary>
    public partial class ExpenseEditWindow : Window
    {
        ExpenseViewModel viewModel;
        Expense expenseBuff;
        public ExpenseEditWindow(ExpenseViewModel expenseViewModel, Expense item)
        {
            InitializeComponent();

            expenseBuff = item;
            viewModel = expenseViewModel;
            DataContext = viewModel;


            TransacrionTitleBox.Text = item.title;
            TransacrionCategoryComboBox.SelectedItem = item.category;
            TransacrionCurrencyComboBox.SelectedItem = item.currency;
            TransacrionValueBox.Text = Math.Abs(item.value).ToString();
            TransactionAdd.IsChecked = item.value > 0;
            TransactionDate.SelectedDate = item.date;
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

        private void SaveExpense(object sender, RoutedEventArgs e)
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

            expenseBuff.currency.balance -= expenseBuff.value;

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

            this.DialogResult = true;
        }
    }
}
