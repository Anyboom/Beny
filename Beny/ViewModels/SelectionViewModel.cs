using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Beny.Base;
using Beny.Commands;
using Beny.Models;
using Beny.Repositories;
using MvvmDialogs;

namespace Beny.ViewModels
{
    class SelectionViewModel : BindableBase, IModalDialogViewModel
    {
        public bool? DialogResult { get; set; } = false;
        public ObservableCollection<Tag> LeftItems { get; set; }
        public ObservableCollection<Tag> RigthItems { get; set; }
        public Tag SelectedLeftItem { get; set; }
        public Tag SelectedRigthItem { get; set; }

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
            LeftItems = new ObservableCollection<Tag>(_mainRepository.Tags.Local.Except(RigthItems));

            OnPropertyChanged(nameof(RigthItems));
            OnPropertyChanged(nameof(LeftItems));
        }
    }
}
