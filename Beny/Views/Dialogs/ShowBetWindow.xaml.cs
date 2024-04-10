using Beny.ViewModels;
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

namespace Beny.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ShowBetWindow.xaml
    /// </summary>
    public partial class ShowBetWindow : Window
    {
        public ShowBetWindow(ShowBetViewModel betViewModel)
        {
            InitializeComponent();

            DataContext = betViewModel;
        }
    }
}
