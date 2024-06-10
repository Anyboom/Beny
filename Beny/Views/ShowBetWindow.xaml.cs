using Beny.Utilities;
using System.Windows;

namespace Beny.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ShowBetWindow.xaml
    /// </summary>
    public partial class ShowBetWindow : Window
    {
        public ShowBetWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
    }
}
