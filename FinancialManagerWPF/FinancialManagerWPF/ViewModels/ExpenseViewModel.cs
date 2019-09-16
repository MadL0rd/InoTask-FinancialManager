using FinancialManagerWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FinancialManagerWPF.ViewModels
{
    class ExpenseViewModel : DependencyObject
    {
        public ExpenseViewModel(ExpenseContext db)
        {
            Expenses = CollectionViewSource.GetDefaultView(db.expenses.Local);
            Expenses.Filter = FilterExpense;
            Currencies = CollectionViewSource.GetDefaultView(db.currencies.Local);
        }



        //Collections
        public ICollectionView Expenses
        {
            get { return (ICollectionView)GetValue(ExpensesProperty); }
            set { SetValue(ExpensesProperty, value); }
        }
        public static readonly DependencyProperty ExpensesProperty =
            DependencyProperty.Register("Expenses", typeof(ICollectionView), typeof(ExpenseViewModel), new PropertyMetadata(null));


        public ICollectionView Currencies
        {
            get { return (ICollectionView)GetValue(CurrenciesProperty); }
            set { SetValue(CurrenciesProperty, value); }
        }
        public static readonly DependencyProperty CurrenciesProperty =
            DependencyProperty.Register("Currencies", typeof(ICollectionView), typeof(ExpenseViewModel), new PropertyMetadata(null));



        //Filters
        public Currency CurrencyFilter
        {
            get { return (Currency)GetValue(CurrencyFilterProperty); }
            set { SetValue(CurrencyFilterProperty, value); }
        }
        public static readonly DependencyProperty CurrencyFilterProperty =
            DependencyProperty.Register("CurrencyFilter", typeof(Currency), typeof(ExpenseViewModel), new PropertyMetadata(null, Filter_Changed));
                
        public Category CategoryFilter
        {
            get { return (Category)GetValue(CategoryFilterProperty); }
            set { SetValue(CategoryFilterProperty, value); }
        }
        public static readonly DependencyProperty CategoryFilterProperty =
            DependencyProperty.Register("CategoryFilter", typeof(Category), typeof(ExpenseViewModel), new PropertyMetadata(null, Filter_Changed));


        private static void Filter_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var current = d as ExpenseViewModel;
            if (current != null)
            {
                current.Expenses.Filter = null;
                current.Expenses.Filter = current.FilterExpense;
            }
        }        
        private bool FilterExpense(object obj)
        {
            Expense item = obj as Expense;
            if (item != null)
            {
                if (CurrencyFilter != null && item.currency.id != CurrencyFilter.id)
                {
                    return false;
                }
                if (CategoryFilter != null && item.category.id != CategoryFilter.id)
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
