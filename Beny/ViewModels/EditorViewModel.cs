using Beny.Base;
using Beny.Commands;
using Beny.Models.Interfaces;
using Beny.Repositories;
using Microsoft.EntityFrameworkCore;
using MvvmDialogs;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Beny.ViewModels
{
    public class EditorViewModel<T> : BindableBase, IModalDialogViewModel where T: class, IDictionaryModel, new()
    {
        private readonly MainRepository _mainRepository;
        private readonly IDialogService _dialogService;
        private readonly CollectionView _collectionView;

        private T _selectedItem;

        public bool? DialogResult { get; set; } = false;

        public ObservableCollection<T> Items { get; set; }

        private string _searchText = string.Empty;

        public string Title { get; set; }

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;

                OnPropertyChanged(nameof(SearchText));

                _collectionView.Refresh();
            }
        }

        public T SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;

                InputItem = value?.Name!;

                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(InputItem));
            }
        }

        public string InputItem { get; set; } = string.Empty;

        public ICommand AddItemCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand SaveItemsCommand { get; set; }
        public ICommand ClosedWindowCommand { get; set; }

        public EditorViewModel(MainRepository mainRepository, IDialogService dialogService)
        {
            _mainRepository = mainRepository;
            _dialogService = dialogService;

            Items = _mainRepository.Set<T>().Local.ToObservableCollection();

            _collectionView = (CollectionView) CollectionViewSource.GetDefaultView(Items);

            _collectionView.Filter = FilterTeamByName;

            AddItemCommand = new RelayCommand(AddItem, x => InputItem?.Length > 0);
            EditItemCommand = new RelayCommand(EditItem, x => InputItem?.Length > 0);
            RemoveItemCommand = new RelayCommand(RemoveItem, x => InputItem?.Length > 0);
            SaveItemsCommand = new RelayCommand(SaveItems);
            ClosedWindowCommand = new RelayCommand(ClosedWindow);

            Title = $"Бени - редактирование модели: {typeof(T).Name}";
        }

        private bool FilterTeamByName(object x)
        {
            T item = (T) x;

            return item.Name.Contains(SearchText) && item.DeletedAt == null;
        }

        private void ClosedWindow(object obj)
        {
            foreach (var entry in _mainRepository.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Detached;
                        entry.Reload();
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }

        private void SaveItems(object obj)
        {
            _mainRepository.SaveChanges();

            DialogResult = true;

            _dialogService.Close(this);
        }

        private void RemoveItem(object obj)
        {
            SelectedItem.DeletedAt = DateTime.Now;

            _collectionView.Refresh();
        }

        private void EditItem(object obj)
        {
            SelectedItem.Name = InputItem;
        }

        private void AddItem(object obj)
        {
            T item = new T()
            {
                Name = InputItem
            };

            _mainRepository.Set<T>().Add(item);
        }
    }
}
