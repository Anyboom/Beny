using System;
using SimpleInjector;
using System.Windows;
using Beny.Services.Interfaces;
using Beny.ViewModels.Base;

namespace Beny.Services
{
    public class DialogService(Container container) : IDialogService
    {
        public void Show<TView, TViewModel>(Action<TViewModel> func)
        {
            Window dialog = container.GetInstance(typeof(TView)) as Window;

            object viewModel = container.GetInstance(typeof(TViewModel));

            func?.Invoke((TViewModel)viewModel);

            dialog.DataContext = viewModel;

            dialog.Show();
        }

        public void Show<TView>()
        {
            Window dialog = container.GetInstance(typeof(TView)) as Window;

            dialog.Show();
        }

        public void Show<TView, TViewModel>()
        {
            Window dialog = container.GetInstance(typeof(TView)) as Window;

            object viewModel = container.GetInstance(typeof(TViewModel));

            dialog.DataContext = viewModel;

            dialog.Show();
        }

        public void ShowDialog<TView, TViewModel>(Action<TViewModel> func)
        {
            Window dialog = container.GetInstance(typeof(TView)) as Window;

            object viewModel = container.GetInstance(typeof(TViewModel));

            func?.Invoke((TViewModel)viewModel);

            dialog.DataContext = viewModel;

            dialog.ShowDialog();

        }

        public void ShowDialog<TView>()
        {
            Window dialog = container.GetInstance(typeof(TView)) as Window;

            dialog.ShowDialog();
        }

        public void ShowDialog<TView, TViewModel>()
        {
            Window dialog = container.GetInstance(typeof(TView)) as Window;

            object viewModel = container.GetInstance(typeof(TViewModel));

            dialog.DataContext = viewModel;

            dialog.ShowDialog();
        }
    }
}

