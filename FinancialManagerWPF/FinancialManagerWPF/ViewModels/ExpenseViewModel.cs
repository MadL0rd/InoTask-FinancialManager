using FinancialManagerWPF.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FinancialManagerWPF.ViewModels
{
    public class ExpenseViewModel : DependencyObject
    {
        public ExpenseViewModel(ExpenseContext db)
        {
            Expenses = CollectionViewSource.GetDefaultView(db.expenses.Local);
            Currencies = CollectionViewSource.GetDefaultView(db.currencies.Local);
            Categories = CollectionViewSource.GetDefaultView(db.categories.Local);

            Expenses.SortDescriptions.Add(new SortDescription("date", ListSortDirection.Ascending));
            Currencies.SortDescriptions.Add(new SortDescription("title", ListSortDirection.Ascending));
            Categories.SortDescriptions.Add(new SortDescription("title", ListSortDirection.Ascending));

            RefreshDiagram();

            Expenses.Filter = FilterExpense;
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

        //Diagrams



        public bool AddShowingFilter
        {
            get { return (bool)GetValue(AddShowingFilterProperty); }
            set { SetValue(AddShowingFilterProperty, value); }
        }
        public static readonly DependencyProperty AddShowingFilterProperty =
            DependencyProperty.Register("AddShowingFilter", typeof(bool), typeof(ExpenseViewModel), new PropertyMetadata(false, Filter_Changed));


        public SeriesCollection Series
        {
            get { return (SeriesCollection)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(SeriesCollection), typeof(ExpenseViewModel), new PropertyMetadata(null));

        public List<string> Labels
        {
            get { return (List<string>)GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }
        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(List<string>), typeof(ExpenseViewModel), new PropertyMetadata(null));


        private void RefreshDiagram()
        {
            Labels = new List<string>();
            Series = new SeriesCollection();
            if (CurrencyFilter == null)
            {
                foreach (Currency item in Currencies)
                {
                    Labels.Add(item.title);
                }
            }
            else
            {
                Labels.Add(CurrencyFilter.title);
            }


            if (CategoryFilter == null)
            {
                foreach (Category item in Categories)
                {
                    ColumnSeries columnSeries = new ColumnSeries
                    {
                        Title = item.title,
                        Values = new ChartValues<double>(),
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(item.color)
                    };
                    for (int i = 0; i < Labels.Count; i++) columnSeries.Values.Add(0.0);
                    Series.Add(columnSeries);
                }
            }
            else
            {
                ColumnSeries columnSeries = new ColumnSeries
                {
                    Title = CategoryFilter.title,
                    Values = new ChartValues<double>(),
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(CategoryFilter.color)
                };
                for (int i = 0; i < Labels.Count; i++) columnSeries.Values.Add(0.0);
                Series.Add(columnSeries);
            }
        }
        private static void Filter_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var current = d as ExpenseViewModel;
            if (current != null)
            {
                current.RefreshDiagram();

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


                for (int i = 0; i < Series.Count; i++)
                {
                    if (Series[i].Title == item.category.title)
                    {
                        for (int j = 0; j < Labels.Count; j++)
                        {
                            if (Labels[j] == item.currency.title)
                            {
                                if (item.value < 0 && !AddShowingFilter)
                                {
                                    Series[i].Values[j] = Convert.ToDouble(Series[i].Values[j]) - item.value;
                                }
                                if (item.value > 0 && AddShowingFilter)
                                {
                                    Series[i].Values[j] = Convert.ToDouble(Series[i].Values[j]) + item.value;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }

                return true;
            }
            return false;
        }
    }
}
