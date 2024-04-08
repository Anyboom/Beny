using Beny.Base;
using Beny.Commands;
using Beny.Interfaces;
using Beny.Models;
using Beny.Enums;
using Beny.Repositories;
using Beny.Services;
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

namespace Beny.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region [Private variables]

        private MainRepository _mainRepository;
        private IDialogService _dialogService;
        private CollectionView _collectionView;

        private string _selectedYear;
        private KeyValuePair<int, string> _selectedMonth;

        #endregion

        #region [Getters/Setters]

        public ObservableCollection<Bet> Bets { get; set; }
        public DateTime StartRange { get; set; } = DateTime.Now.AddDays(-4);
        public DateTime EndRange { get; set; } = DateTime.Now.AddDays(4);
        public Bet SelectedBet { get; set; }

        public List<string> YearsList { get; set; } = new List<string>();

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
        public ICommand UpdateTableWithDateCommand { get; set; }

        #endregion

        #region [MainViewModel]

        public MainViewModel(MainRepository mainRepository, IDialogService dialogService)
        {
            this._mainRepository = mainRepository;
            this._dialogService = dialogService;

            EditBetCommand = new RelayCommand(EditBet);
            AddBetCommand = new RelayCommand(AddBet);
            UpdateTableWithDateCommand = new RelayCommand(UpdateTableWithDate);
            LoadedWindowCommand = new RelayCommand(LoadedWindow);
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

            _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Bets);

            _collectionView.SortDescriptions.Add(new SortDescription(nameof(Bet.CreatedAt), ListSortDirection.Descending));

            YearsList.Add("Все");
            YearsList.AddRange(_mainRepository.FootballEvents.Select(x => x.CreatedAt.Year.ToString()).Distinct());

            SelectedYear = DateTime.Now.Year.ToString();
            SelectedMonth = MonthsList.First(x => x.Key == DateTime.Now.Month);

            OnPropertyChanged(nameof(Bets));

            UpdateTableWithDate();
        }

        private void UpdateTableWithDate(object x = null)
        {
            _collectionView.Filter = (x) =>
            {
                Bet bet = (Bet)x;

                bool yearCondition = SelectedYear == "Все" || bet.CreatedAt.Year == Convert.ToInt32(SelectedYear);
                bool monthCondition = SelectedMonth.Value == "Все" || bet.CreatedAt.Month == SelectedMonth.Key;

                return yearCondition & monthCondition;
            };
        }

        private void EditBet(object x)
        {
            _dialogService.ShowDialog<BetWindow, BetViewModel>(x => x.UpdateBetId = SelectedBet.Id);
        }

        private void AddBet(object x)
        {
            _dialogService.ShowDialog<BetWindow, BetViewModel>();
        }

        #endregion
    }
}
