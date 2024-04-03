using System.Windows;
using Beny.Repositories;
using Beny.Services;
using Beny.Services.Interfaces;
using Beny.ViewModels;
using Beny.Views.Dialogs;
using Beny.Views.Windows;
using Microsoft.EntityFrameworkCore;
using SimpleInjector;

namespace Beny
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Container container = new Container();

            container.Register<IDialogService, DialogService>();

            //container.Register<DbContext, MainRepository>(Lifestyle.Scoped);

            container.Register<MainRepository>(Lifestyle.Singleton);
            container.Register<ErrorsViewModel>();

            container.Register<MainViewModel>();
            container.Register<MainWindow>();

            container.Register<BetViewModel>();
            container.Register<BetWindow>();

            container.Verify();

            App app = new App();

            Window window = container.GetInstance<MainWindow>();

            app.Run(window);
        }
    }
}
