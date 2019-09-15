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
    /// Логика взаимодействия для GetNewTitleWindow.xaml
    /// </summary>
    public partial class GetNewTitleWindow : Window
    {
        public GetNewTitleWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (TitleBox.Text.Length != 0)
            {
                this.DialogResult = true;
            }
        }

        public string Title
        {
            get { return TitleBox.Text; }
        }
    }
}
