using Beny.Base;
using Beny.Commands;
using Beny.Models;
using Beny.Enums;
using Beny.Repositories;
using Beny.Views.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.MessageBox;

namespace Beny.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region [Private variables]

        private readonly MainRepository _mainRepository;
        private readonly IDialogService _dialogService;
        private readonly EditBetViewModel _editBetViewModel;

        private readonly EditorViewModel<Tag> _tagEditorViewModel;
        private readonly EditorViewModel<Team> _teamEditorViewModel;
        private readonly EditorViewModel<Forecast> _forecastEditorViewModel;
        private readonly EditorViewModel<Sport> _sportEditorViewModel;
        private readonly EditorViewModel<Competition> _competitionEditorViewModel;

        private readonly ShowBetViewModel _showBetViewModel;

        private CollectionView _collectionView;

        private string _selectedYear;
        private KeyValuePair<int, string> _selectedMonth;

        #endregion

        #region [Getters/Setters]

        public ObservableCollection<Bet> Bets { get; set; }
        public Bet SelectedBet { get; set; }
        public List<string> YearsList { get; set; } = new() { "Все" };

        public int? CountWin
        {
            get
            {
                return Bets?.Where(FilterByDateTime).Count(x => x.Status == FootballEventStatus.Win);
            }
        }

        public int? CountLose
        {
            get
            {
                return Bets?.Where(FilterByDateTime).Count(x => x.Status == FootballEventStatus.Lose);
            }
        }

        public int? CountReturn
        {
            get
            {
                return Bets?.Where(FilterByDateTime).Count(x => x.Status == FootballEventStatus.Return);
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
        public ICommand ShowSportsEditorCommand { get; set; }
        public ICommand ShowForecastsEditorCommand { get; set; }
        public ICommand ShowCompetitionsEditorCommand { get; set; }
        public ICommand ShowTagsEditorCommand { get; set; }

        #endregion

        #region [MainViewModel]

        public MainViewModel(MainRepository mainRepository, IDialogService dialogService, EditBetViewModel editBetViewModel, EditorViewModel<Tag> tagEditorViewModel, EditorViewModel<Forecast> forecastEditorViewModel, EditorViewModel<Sport> sportEditorViewModel, EditorViewModel<Team> teamEditorViewModel, EditorViewModel<Competition> competitionEditorViewModel, ShowBetViewModel showBetViewModel) 
        {
            _mainRepository = mainRepository;
            _dialogService = dialogService;
            _editBetViewModel = editBetViewModel;

            _sportEditorViewModel = sportEditorViewModel;
            _forecastEditorViewModel = forecastEditorViewModel;
            _competitionEditorViewModel = competitionEditorViewModel;
            _tagEditorViewModel = tagEditorViewModel;
            _teamEditorViewModel = teamEditorViewModel;

            _showBetViewModel = showBetViewModel;

            EditBetCommand = new RelayCommand(EditBet, _ => SelectedBet != null);
            AddBetCommand = new RelayCommand(AddBet);
            UpdateTableWithDateCommand = new RelayCommand(UpdateTableWithDate);
            LoadedWindowCommand = new RelayCommand(LoadedWindow);
            DeleteBetCommand = new RelayCommand(DeleteBet, _ => SelectedBet != null);
            ShowBetCommand = new RelayCommand(ShowBet, _ => SelectedBet != null);
            ShowTeamsEditorCommand = new RelayCommand(ShowTeamsEditor);
            ShowTagsEditorCommand = new RelayCommand(ShowTagsEditor);
            ShowSportsEditorCommand = new RelayCommand(ShowSportsEditor);
            ShowForecastsEditorCommand = new RelayCommand(ShowForecastsEditor);
            ShowCompetitionsEditorCommand = new RelayCommand(ShowCompetitionsEditor);
        }

        private void ShowTagsEditor(object obj)
        {
            EditorViewModel<Tag> viewModel = _tagEditorViewModel;

            bool? result = _dialogService.ShowDialog<EditorWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void ShowCompetitionsEditor(object obj)
        {
            EditorViewModel<Competition> viewModel = _competitionEditorViewModel;

            bool? result = _dialogService.ShowDialog<EditorWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void ShowForecastsEditor(object obj)
        {
            EditorViewModel<Forecast> viewModel = _forecastEditorViewModel;

            bool? result = _dialogService.ShowDialog<EditorWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void ShowSportsEditor(object obj)
        {
            EditorViewModel<Sport> viewModel = _sportEditorViewModel;

            bool? result = _dialogService.ShowDialog<EditorWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void ShowTeamsEditor(object obj)
        {
            EditorViewModel<Team> viewModel = _teamEditorViewModel;

            bool? result = _dialogService.ShowDialog<EditorWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void ShowBet(object obj)
        {
            ShowBetViewModel viewModel = _showBetViewModel;

            viewModel.CurrentBet = SelectedBet;

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
            Bets = _mainRepository.Bets.Local.ToObservableCollection();

            _collectionView = (CollectionView) CollectionViewSource.GetDefaultView(Bets);

            _collectionView.SortDescriptions.Add(new SortDescription(nameof(Bet.CreatedAt), ListSortDirection.Descending));

            YearsList.AddRange(_mainRepository.FootballEvents.Local.Select(x => x.CreatedAt.Year.ToString()).Distinct());

            SelectedYear = (YearsList.Count > 1) ? DateTime.Now.Year.ToString() : YearsList[0];

            SelectedMonth = MonthsList.First(x => x.Key == DateTime.Now.Month);

            UpdateTableWithDate();
        }

        private void UpdateTableWithDate(object x = null)
        {
            _collectionView.Filter = FilterByDateTime;

            OnPropertyChanged(nameof(Bets));

            OnPropertyChanged(nameof(CountLose));
            OnPropertyChanged(nameof(CountReturn));
            OnPropertyChanged(nameof(CountWin));
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

            return yearCondition && monthCondition;
        }

        private void EditBet(object x)
        {
            EditBetViewModel viewModel = _editBetViewModel;

            viewModel.UpdateBetId = SelectedBet.Id;

            bool? result = _dialogService.ShowDialog<CreateOrUpdateBetWindow>(this, viewModel);

            if (result == true)
            {
                UpdateProperties();
            }
        }

        private void AddBet(object x)
        {
            EditBetViewModel viewModel = _editBetViewModel;

            viewModel.UpdateBetId = -1;

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

            _collectionView.Refresh();
        }

        #endregion
    }
}
