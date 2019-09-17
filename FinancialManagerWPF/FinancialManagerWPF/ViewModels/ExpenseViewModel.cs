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
    public class ExpenseViewModel : DependencyObject
    {
        public ExpenseViewModel(ExpenseContext db)
        {
            Expenses = CollectionViewSource.GetDefaultView(db.expenses.Local);
            Expenses.Filter = FilterExpense;
            Currencies = CollectionViewSource.GetDefaultView(db.currencies.Local);
            Categories = CollectionViewSource.GetDefaultView(db.categories.Local);

            Expenses.SortDescriptions.Add(new SortDescription("date", ListSortDirection.Ascending));
            Currencies.SortDescriptions.Add(new SortDescription("title", ListSortDirection.Ascending));
            Categories.SortDescriptions.Add(new SortDescription("title", ListSortDirection.Ascending));
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


        public ICollectionView Categories
        {
            get { return (ICollectionView)GetValue(CategoriesProperty); }
            set { SetValue(CategoriesProperty, value); }
        }
        public static readonly DependencyProperty CategoriesProperty =
            DependencyProperty.Register("Categories", typeof(ICollectionView), typeof(ExpenseViewModel), new PropertyMetadata(null));




        //Filters


        public string TitleFilter
        {
            get { return (string)GetValue(TitleFilterProperty); }
            set { SetValue(TitleFilterProperty, value); }
        }
        public static readonly DependencyProperty TitleFilterProperty =
            DependencyProperty.Register("TitleFilter", typeof(string), typeof(ExpenseViewModel), new PropertyMetadata(null, Filter_Changed));

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

        private static DateTime GetMonthBeginDate()
        {
            DateTime date = DateTime.Today;
            date = date.AddDays(-1 * DateTime.Today.Day + 1);
            return date;
        }
        public DateTime BeginDateFilter
        {
            get { return (DateTime)GetValue(BeginDateFilterProperty); }
            set { SetValue(BeginDateFilterProperty, value); }
        }
        public static readonly DependencyProperty BeginDateFilterProperty =
            DependencyProperty.Register("BeginDateFilter", typeof(DateTime), typeof(ExpenseViewModel), new PropertyMetadata(GetMonthBeginDate(), Filter_Changed));

        public DateTime EndDateFilter
        {
            get { return (DateTime)GetValue(EndDateFilterProperty); }
            set { SetValue(EndDateFilterProperty, value); }
        }
        public static readonly DependencyProperty EndDateFilterProperty =
            DependencyProperty.Register("EndDateFilter", typeof(DateTime), typeof(ExpenseViewModel), new PropertyMetadata(DateTime.Today, Filter_Changed));




        private static void Filter_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var current = d as ExpenseViewModel;
            if (current != null)
            {
                current.Expenses.Filter = null;
                current.Expenses.Filter = current.FilterExpense;
            }
        }
        public void ResetFilters()
        {
            CurrencyFilter = null;
            CategoryFilter = null;
            BeginDateFilter = GetMonthBeginDate();
            EndDateFilter = DateTime.Today;
            TitleFilter = null;
        }
        private bool FilterExpense(object obj)
        {
            Expense item = obj as Expense;
            if (item != null)
            {
                if (CurrencyFilter != null && item.currency != CurrencyFilter)
                {
                    return false;
                }
                if (CategoryFilter != null && item.category != CategoryFilter)
                {
                    return false;
                }
                if (item.date < BeginDateFilter || item.date > EndDateFilter)
                {
                    return false;
                }
                if (TitleFilter != null && TitleFilter.Length != 0 && !item.title.ToLower().Contains(TitleFilter.ToLower()))
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
