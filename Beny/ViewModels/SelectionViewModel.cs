using System.Collections.ObjectModel;
using System.Windows.Input;
using Beny.Base;
using Beny.Commands;
using Beny.Repositories;
using MvvmDialogs;

namespace Beny.ViewModels
{
    public class SelectionViewModel<T> : BindableBase, IModalDialogViewModel where T: class, new ()
    {
        public bool? DialogResult { get; set; } = false;
        public ObservableCollection<T> LeftItems { get; set; }
        public ObservableCollection<T> RigthItems { get; set; }
        public T SelectedLeftItem { get; set; }
        public T SelectedRigthItem { get; set; }

        private readonly MainRepository _mainRepository;
        private readonly IDialogService _dialogService;

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ToRigthCommand { get; set; }
        public ICommand ToLeftCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public SelectionViewModel(MainRepository mainRepository, IDialogService dialogService)
        {
            _mainRepository = mainRepository;
            _dialogService = dialogService;

            LoadedWindowCommand = new RelayCommand(LoadedWindow);
            ToRigthCommand = new RelayCommand(ToRigth, x => SelectedLeftItem != null);
            ToLeftCommand = new RelayCommand(ToLeft, x => SelectedRigthItem != null);
            SaveCommand = new RelayCommand(Save);
        }

        private void Save(object obj)
        {
            DialogResult = true;

            _dialogService.Close(this);
        }

        private void ToLeft(object obj)
        {
            LeftItems.Add(SelectedRigthItem);
            RigthItems.Remove(SelectedRigthItem);
        }

        private void ToRigth(object obj)
        {
            RigthItems.Add(SelectedLeftItem);
            LeftItems.Remove(SelectedLeftItem);
        }

        private void LoadedWindow(object obj)
        {
            LeftItems = new ObservableCollection<T>(_mainRepository.Set<T>().Local.Except(RigthItems));

            OnPropertyChanged(nameof(RigthItems));
            OnPropertyChanged(nameof(LeftItems));
        }
    }
}
