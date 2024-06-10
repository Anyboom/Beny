using Beny.Utilities;
using System.Windows;

namespace Beny.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для BetWindow.xaml
    /// </summary>
    public partial class CreateOrUpdateBetWindow : Window
    {
        public CreateOrUpdateBetWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
    }
}
