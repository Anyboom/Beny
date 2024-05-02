using System.Windows;
using Beny.Models;
using Beny.Repositories;
using Beny.ViewModels;
using Beny.Views.Dialogs;
using Beny.Views.Windows;
using Microsoft.EntityFrameworkCore;
using MvvmDialogs;
using SimpleInjector;

namespace Beny
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Container container = new Container();

            container.Register<IDialogService>(() => new DialogService(), Lifestyle.Singleton);

            container.Register<MainRepository>(Lifestyle.Singleton);

            container.Register<MainViewModel>();
            container.Register<MainWindow>();

            container.Register<EditBetViewModel>();
            container.Register<CreateOrUpdateBetWindow>();

            container.Register<EditorViewModel<Sport>>();
            container.Register<EditorViewModel<Competition>>();
            container.Register<EditorViewModel<Team>>();
            container.Register<EditorViewModel<Forecast>>();

            container.Register<EditorWindow>();

            container.Register<ShowBetWindow>();

            container.Verify();

            App app = new App();

            Window window = container.GetInstance<MainWindow>();

            app.Run(window);
        }
    }
}
