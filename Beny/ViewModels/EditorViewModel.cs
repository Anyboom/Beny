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
using SimpleInjector;

namespace Beny.ViewModels
{
    public class EditorViewModel<T> : BindableBase, IModalDialogViewModel where T : class
    {
        private readonly Container _container;
        private readonly MainRepository _mainRepository;
        private T _selectedItem;

        public bool? DialogResult => false;

        public ObservableCollection<T> Items { get; set; }

        public T SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;

                InputItem = value.ToString();

                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(InputItem));
            }
        }

        public string InputItem { get; set; }

        public RelayCommand AddItemCommand { get; set; }
        public RelayCommand EditItemCommand { get; set; }
        public RelayCommand RemoveItemCommand { get; set; }
        public RelayCommand SaveItemsCommand { get; set; }

        public EditorViewModel(Container container)
        {
            _container = container;

            _mainRepository = _container.GetInstance<MainRepository>();

            Items = _mainRepository.Set<T>().Local.ToObservableCollection();

            AddItemCommand = new RelayCommand(AddItem);
            EditItemCommand = new RelayCommand(EditItem);
            RemoveItemCommand = new RelayCommand(RemoveItem);
            SaveItemsCommand = new RelayCommand(SaveItems);
        }

        private void SaveItems(object obj)
        {
            _mainRepository.SaveChanges();
        }

        private void RemoveItem(object obj)
        {
            _mainRepository.Set<T>().Remove(SelectedItem);
        }

        private void EditItem(object obj)
        {
            throw new NotImplementedException();
        }

        private void AddItem(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
