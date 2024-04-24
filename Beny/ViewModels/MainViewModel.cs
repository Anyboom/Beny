using Beny.Base;
using Beny.Commands;
using Beny.Models;
using Beny.Enums;
using Beny.Repositories;
using Beny.Views.Dialogs;
using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Beny.Collections;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using Container = SimpleInjector.Container;

namespace Beny.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region [Private variables]

        private Container _container;
        private MainRepository _mainRepository;
        private IDialogService _dialogService;
        private CollectionView _collectionView;

        private string _selectedYear;
        private KeyValuePair<int, string> _selectedMonth;

        #endregion

        #region [Getters/Setters]

        public ObservableCollection<Bet> Bets { get; set; }
        public Bet SelectedBet { get; set; }
        public List<string> YearsList { get; set; } = new List<string>();

        public int CountWin
        {
            get
            {
                if (Bets == null)
                {
                    return 0;
                }

                return Bets.Where(FilterByDateTime).Count(x => x.Status == FootballEventStatus.Win);
            }
        }

        public int CountLose
        {
            get
            {
                if (Bets == null)
                {
                    return 0;
                }

                return Bets.Where(FilterByDateTime).Count(x => x.Status == FootballEventStatus.Lose);
            }
        }

        public int CountReturn
        {
            get
            {
                if (Bets == null)
                {
                    return 0;
                }

                return Bets.Where(FilterByDateTime).Count(x => x.Status == FootballEventStatus.Return);
            }
        }

        public string SelectedYear
        {
            get
            {
                return _selectedYear;
            }
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
            }
        }

        public KeyValuePair<int, string> SelectedMonth
        {
            get
            {
                return _selectedMonth;
            }
            set
            {
                _selectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth));
            }
        }

        public Dictionary<int, string> MonthsList { get; set; } = new Dictionary<int, string>()
        {
            { 0, "Все"},
            { 1, "Январь"},
            { 2, "Февраль"},
            { 3, "Март"},
            { 4, "Апрель"},
            { 5, "Май"},
            { 6, "Июнь"},
            { 7, "Июль"},
            { 8, "Август"},
            { 9, "Сентябрь"},
            { 10, "Октябрь"},
            { 11, "Ноябрь"},
            { 12, "Декабрь"},
        };

        #endregion

        #region [Commands]

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand EditBetCommand { get; set; }
        public ICommand AddBetCommand { get; set; }
        public ICommand DeleteBetCommand { get; set; }
        public ICommand ShowBetCommand { get; set; }
        public ICommand UpdateTableWithDateCommand { get; set; }
        public ICommand ShowTeamsEditorCommand { get; set; }

        #endregion

        #region [MainViewModel]

        public MainViewModel(Container container)
        {
            _container = container;

            _mainRepository = _container.GetInstance<MainRepository>();
            _dialogService = _container.GetInstance<IDialogService>();

            EditBetCommand = new RelayCommand(EditBet, _ => SelectedBet != null);
            AddBetCommand = new RelayCommand(AddBet);
            UpdateTableWithDateCommand = new RelayCommand(UpdateTableWithDate);
            LoadedWindowCommand = new RelayCommand(LoadedWindow);
            DeleteBetCommand = new RelayCommand(DeleteBet, _ => SelectedBet != null);
            ShowBetCommand = new RelayCommand(ShowBet, _ => SelectedBet != null);
            ShowTeamsEditorCommand = new RelayCommand(ShowTeamsEditor);
        }

        private void ShowTeamsEditor(object obj)
        {
            EditorViewModel<Sport> viewModel = _container.GetInstance<EditorViewModel<Sport>>();

            _dialogService.ShowDialog<EditorWindow>(this, viewModel);
        }

        private void ShowBet(object obj)
        {
            ShowBetViewModel viewModel = new ShowBetViewModel()
            {
                CurrentBet = SelectedBet
            };

            _dialogService.Show<ShowBetWindow>(this, viewModel);
        }

        private void DeleteBet(object obj)
        {
            var messageBoxSettings = new MessageBoxSettings()
            {
                Button = MessageBoxButton.YesNo,
                DefaultResult = MessageBoxResult.No,
                Icon = MessageBoxImage.Question,
                Caption = $"Ставка #{SelectedBet.Id}",
                MessageBoxText = "Вы уверены в том, что хотите удалить эту ставку ?"
            };

            MessageBoxResult result = _dialogService.ShowMessageBox(this, messageBoxSettings);

            if (result == MessageBoxResult.Yes)
            {
                _mainRepository.Bets.Remove(SelectedBet);
                _mainRepository.SaveChanges();

                UpdateProperties();
            }
        }

        private void LoadedWindow(object x)
        {
            _mainRepository.Database.EnsureCreated();

            _mainRepository.Bets.Load();
            _mainRepository.FootballEvents.Load();
            _mainRepository.Teams.Load();
            _mainRepository.Forecasts.Load();
            _mainRepository.Sports.Load();
            _mainRepository.Competitions.Load();

            Bets = _mainRepository.Bets.Local.ToObservableCollection();

            _collectionView = (CollectionView) CollectionViewSource.GetDefaultView(Bets);

            _collectionView.SortDescriptions.Add(new SortDescription(nameof(Bet.CreatedAt), ListSortDirection.Descending));

            YearsList.Add("Все");
            YearsList.AddRange(_mainRepository.FootballEvents.Select(x => x.CreatedAt.Year.ToString()).Distinct());

            SelectedYear = DateTime.Now.Year.ToString();
            SelectedMonth = MonthsList.First(x => x.Key == DateTime.Now.Month);

            UpdateTableWithDate();
        }

        private void UpdateTableWithDate(object x = null)
        {
            _collectionView.Filter = FilterByDateTime;

            UpdateProperties();
        }

        private bool FilterByDateTime(object x)
        {
            Bet bet = (Bet) x;

            return FilterByDateTime(bet);
        }

        private bool FilterByDateTime(Bet bet)
        {
            bool yearCondition = SelectedYear == "Все" || bet.CreatedAt.Year == Convert.ToInt32(SelectedYear);
            bool monthCondition = SelectedMonth.Value == "Все" || bet.CreatedAt.Month == SelectedMonth.Key;

            return yearCondition & monthCondition;
        }

        private void EditBet(object x)
        {
            var viewModel = _container.GetInstance<CreateOrUpdateBetViewModel>();

            viewModel.UpdateBetId = SelectedBet.Id;

            bool? result = _dialogService.ShowDialog<CreateOrUpdateBetWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void AddBet(object x)
        {
            var viewModel = _container.GetInstance<CreateOrUpdateBetViewModel>();
            
            bool? result = _dialogService.ShowDialog<CreateOrUpdateBetWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(Bets));

            OnPropertyChanged(nameof(CountLose));
            OnPropertyChanged(nameof(CountReturn));
            OnPropertyChanged(nameof(CountWin));
        }

        #endregion
    }
}
