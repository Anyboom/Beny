using Beny.Base;
using Beny.Commands;
using Beny.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Beny.Collections;
using Beny.Views;
using MvvmDialogs;
using System.Collections.Generic;
using Beny.Models.Contexts;
using Beny.Models.Enums;

namespace Beny.ViewModels
{
    public class EditBetViewModel : NotifyPropertyChanged, INotifyDataErrorInfo, IModalDialogViewModel
    {
        #region Private variables

        private readonly ErrorsViewModel _errorsViewModel;
        private readonly MainContext _mainContext;
        private readonly IDialogService _dialogService;
        private readonly SelectionViewModel<Tag> _selectionViewModel;

        private string _homeTeam;
        private string _guestTeam;
        private string _competition;
        private string _forecast;
        private string _sport;
        private string _coefficient;
        private int _minute;
        private int _hour;
        private bool _isLive = false;

        private FootballEvent _selectedFootballEvent;

        #endregion

        #region Getters/Setters

        public HashSet<string> TeamList { get; set; }
        public HashSet<string> ForecastList { get; set; }
        public HashSet<string> SportList { get; set; }
        public HashSet<string> CompetitionList { get; set; }

        public int[] AllMinutes { get; set; }
        public int[] AllHours { get; set; }

        public int UpdateBetId { get; set; } = 0;
        public Bet CurrentBet { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }

        public string Title => (UpdateBetId > 0) ? "Бени - редактирование ставки" : "Бени - создание ставки";

        public FootballEvent SelectedFootballEvent
        {
            get
            {
                return _selectedFootballEvent;
            }
            set
            {
                _selectedFootballEvent = value;

                if (value != null)
                {
                    Sport = value.Sport.Name;
                    Coefficient = value.Coefficient.ToString();
                    Date = value.StartedAt.Date;
                    Hour = value.StartedAt.Hour;
                    Minute = value.StartedAt.Minute;
                    HomeTeam = value.HomeTeam.Name;
                    GuestTeam = value.GuestTeam.Name;
                    Competition = value.Competition.Name;
                    Forecast = value.Forecast.Name;

                    Tags = new ObservableCollection<Tag>(value.FootballEventTags.Select(x => x.Tag));

                    IsLive = value.IsLive;
                }

                OnPropertyChanged(nameof(SelectedFootballEvent));
            }
        }

        public string HomeTeam
        {
            get
            {
                return _homeTeam;
            }
            set
            {
                _homeTeam = value;

                _errorsViewModel.ClearErrors(nameof(HomeTeam));

                if (string.IsNullOrWhiteSpace(_homeTeam))
                {
                    _errorsViewModel.AddError(nameof(HomeTeam), "Поле обязательно для заполнения.");
                }

                OnPropertyChanged(nameof(HomeTeam));
            }
        }

        public bool IsLive
        {
            get
            {
                return _isLive;
            }
            set
            {
                _isLive = value;

                OnPropertyChanged(nameof(IsLive));
            }
        }

        public string GuestTeam
        {
            get
            {
                return _guestTeam;
            }
            set
            {
                _guestTeam = value;

                _errorsViewModel.ClearErrors(nameof(GuestTeam));

                if (string.IsNullOrWhiteSpace(_guestTeam))
                {
                    _errorsViewModel.AddError(nameof(GuestTeam), "Поле обязательно для заполнения.");
                }

                OnPropertyChanged(nameof(GuestTeam));
            }
        }

        public string Competition
        {
            get
            {
                return _competition;
            }
            set
            {
                _competition = value;

                _errorsViewModel.ClearErrors(nameof(Competition));

                if (string.IsNullOrWhiteSpace(_competition))
                {
                    _errorsViewModel.AddError(nameof(Competition), "Поле обязательно для заполнения.");
                }

                OnPropertyChanged(nameof(Competition));
            }
        }

        public string Coefficient
        {
            get
            {
                return _coefficient;
            }
            set
            {
                _coefficient = value;

                _errorsViewModel.ClearErrors(nameof(Coefficient));

                if (string.IsNullOrWhiteSpace(_coefficient))
                {
                    _errorsViewModel.AddError(nameof(Coefficient), "Поле обязательно для заполнения.");
                }

                if (float.TryParse(_coefficient, out float _) == false)
                {
                    _errorsViewModel.AddError(nameof(Coefficient), "Поле должно содержать только число или число с разделителем в виде запятой.");
                }

                OnPropertyChanged(nameof(Coefficient));
            }
        }

        public string Forecast
        {
            get
            {
                return _forecast;
            }
            set
            {
                _forecast = value;

                _errorsViewModel.ClearErrors(nameof(Forecast));

                if (string.IsNullOrWhiteSpace(_forecast))
                {
                    _errorsViewModel.AddError(nameof(Forecast), "Поле обязательно для заполнения.");
                }

                OnPropertyChanged(nameof(Forecast));
            }
        }

        public string Sport
        {
            get
            {
                return _sport;
            }
            set
            {
                _sport = value.Trim();

                _errorsViewModel.ClearErrors(nameof(Sport));

                if (string.IsNullOrWhiteSpace(_sport))
                {
                    _errorsViewModel.AddError(nameof(Sport), "Поле обязательно для заполнения.");
                }

                OnPropertyChanged(nameof(Sport));
            }
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;

                OnPropertyChanged(nameof(Date));
            }
        }
        public int Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;

                OnPropertyChanged(nameof(Hour));
            }
        }


        public int Minute
        {
            get
            {
                return _minute;
            }
            set
            {
                _minute = value;

                OnPropertyChanged(nameof(Minute));
            }
        }

        #endregion

        #region Commands

        public ICommand CreateFootballEventCommand { get; set; }
        public ICommand EditFootballEventCommand { get; set; }
        public ICommand DeleteFootballEventCommand { get; set; }
        public ICommand ClearFootballEventCommand { get; set; }
        public ICommand ClosedWindowCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand SaveBetCommand { get; set; }
        public ICommand OpenTagsWindowCommand { get; set; }
        public ICommand UpdateFootballEventStatusCommand { get; set; }

        #endregion

        #region Constructor EditBetViewModel

        public EditBetViewModel(MainContext mainContext, IDialogService dialogService, SelectionViewModel<Tag> selectionViewModel)
        {
            _mainContext = mainContext;
            _dialogService = dialogService;
            _selectionViewModel = selectionViewModel;

            _errorsViewModel = new ErrorsViewModel();

            _errorsViewModel.ErrorsChanged += (s, e) =>
            {
                ErrorsChanged?.Invoke(s, e);
            };

            ForecastList = new HashSet<string>(_mainContext.Forecasts.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));
            SportList = new HashSet<string>(_mainContext.Sports.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));
            CompetitionList = new HashSet<string>(_mainContext.Competitions.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));
            TeamList = new HashSet<string>(_mainContext.Teams.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));

            AllMinutes = Enumerable.Range(0, 60).ToArray();
            AllHours = Enumerable.Range(0, 24).ToArray();

            CreateFootballEventCommand = new RelayCommand(_ => SaveFootballEvent(true), x => CanCreate);
            EditFootballEventCommand = new RelayCommand(_ => SaveFootballEvent(false), x => CanCreate && SelectedFootballEvent != null);
            ClearFootballEventCommand = new RelayCommand(ClearFootballEvent);
            DeleteFootballEventCommand = new RelayCommand(DeleteFootballEvent, x => SelectedFootballEvent != null);

            LoadedWindowCommand = new RelayCommand(LoadedWindow);
            ClosedWindowCommand = new RelayCommand(ClosedWindow);

            SaveBetCommand = new RelayCommand(SaveBet, x => CurrentBet?.FootballEvents.Count > 0);

            UpdateFootballEventStatusCommand = new RelayCommand(UpdateFootballEventStatus, x => SelectedFootballEvent != null);

            OpenTagsWindowCommand = new RelayCommand(OpenTagsWindow);
        } 

        #endregion

        #region OpenTagsWindow

        private void OpenTagsWindow(object obj)
        {
            SelectionViewModel<Tag> viewModel = _selectionViewModel;

            viewModel.RigthItems = new ObservableCollection<Tag>(Tags);

            bool? result = _dialogService.ShowDialog<SelectionWindow>(this, viewModel);

            if (result == true)
            {
                Tags = viewModel.RigthItems;
            }
        }

        #endregion

        private void SaveFootballEvent(bool isNew)
        {
            Competition competition = _mainContext.Competitions.Local.SingleOrDefault(x => x.Name == Competition.Trim(), new Competition() { Name = Competition.Trim() });
            Sport sport = _mainContext.Sports.Local.SingleOrDefault(x => x.Name == Sport.Trim(), new Sport() { Name = Sport.Trim() });
            Forecast forecast = _mainContext.Forecasts.Local.SingleOrDefault(x => x.Name == Forecast.Trim(), new Forecast() { Name = Forecast.Trim() });
            Team homeTeam = _mainContext.Teams.Local.SingleOrDefault(x => x.Name == HomeTeam.Trim(), new Team() { Name = HomeTeam.Trim() });
            Team guestTeam = _mainContext.Teams.Local.SingleOrDefault(x => x.Name == GuestTeam.Trim(), new Team() { Name = GuestTeam.Trim() });

            CompetitionList.Add(competition.Name);
            SportList.Add(sport.Name);
            ForecastList.Add(forecast.Name);
            TeamList.Add(homeTeam.Name);
            TeamList.Add(guestTeam.Name);

            _mainContext.Competitions.Local.Add(competition);
            _mainContext.Sports.Local.Add(sport);
            _mainContext.Forecasts.Local.Add(forecast);
            _mainContext.Teams.Local.Add(homeTeam);
            _mainContext.Teams.Local.Add(guestTeam);

            FootballEvent footballEvent = isNew ? new FootballEvent() { CreatedAt = DateTime.Now } : SelectedFootballEvent;

            footballEvent.StartedAt = new DateTime(Date.Year, Date.Month, Date.Day, Hour, Minute, 0);
            footballEvent.Coefficient = float.Parse(Coefficient);
            footballEvent.FootballEventStatus = FootballEventStatus.NotCalculated;

            footballEvent.Competition = competition;
            footballEvent.Competition.DeletedAt = null;

            footballEvent.Sport = sport;
            footballEvent.Sport.DeletedAt = null;

            footballEvent.Forecast = forecast;
            footballEvent.Forecast.DeletedAt = null;

            footballEvent.HomeTeam = homeTeam;
            footballEvent.HomeTeam.DeletedAt = null;

            footballEvent.GuestTeam = guestTeam;
            footballEvent.GuestTeam.DeletedAt = null;

            footballEvent.IsLive = IsLive;

            footballEvent.FootballEventTags.Clear();

            foreach (Tag tag in Tags)
            {
                footballEvent.FootballEventTags.Add(new FootballEventTag() { Tag = tag });
            }

            if (isNew)
            {
                CurrentBet.FootballEvents.Add(footballEvent);
            }

            OnPropertyChanged(nameof(CurrentBet));
            OnPropertyChanged(nameof(SelectedFootballEvent));
        } 

        #region UpdateFootballEventStatus

        private void UpdateFootballEventStatus(object x)
        {
            FootballEventStatus updateStatus = (FootballEventStatus)x;

            SelectedFootballEvent.FootballEventStatus = updateStatus;
        } 

        #endregion

        #region ClearFootballEvent

        private void ClearFootballEvent(object x)
        {
            Sport = string.Empty;
            Competition = string.Empty;
            HomeTeam = string.Empty;
            GuestTeam = string.Empty;
            Forecast = string.Empty;
            Minute = 10;
            Hour = 0;
            Coefficient = "1,0";
            Date = DateTime.Now;

            Tags.Clear();
        } 

        #endregion

        #region DeleteFootballEvent

        private void DeleteFootballEvent(object x)
        {
            CurrentBet.FootballEvents.Remove(SelectedFootballEvent);
        } 

        #endregion

        #region SaveBet
        private void SaveBet(object x)
        {
            if (UpdateBetId < 1)
            {
                _mainContext.Add(CurrentBet);
            }

            _mainContext.SaveChanges();

            DialogResult = true;

            _dialogService.Close(this);
        } 
        #endregion

        #region LoadedWindow

        private void LoadedWindow(object x)
        {
            CurrentBet = _mainContext.Bets.Local.FirstOrDefault(x => x.Id == UpdateBetId, new Bet() { CreatedAt = DateTime.Now, FootballEvents = new ItemObservableCollection<FootballEvent>() });

            HomeTeam = "Ливерпуль";
            GuestTeam = "Манчестер Сити";
            Forecast = "ТБ 2.5";
            Competition = "Англия. Премьер-лига";
            Coefficient = "1,78";
            Sport = "Футбол";
            Tags = new ObservableCollection<Tag>();
            Date = DateTime.Now;

            _selectedFootballEvent = null;

            OnPropertyChanged(nameof(SelectedFootballEvent));
            OnPropertyChanged(nameof(CurrentBet));
        } 

        #endregion

        #region ClosedWindow

        private void ClosedWindow(object x)
        {
            foreach (var entry in _mainContext.ChangeTracker.Entries())
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
                        if (entry.Entity.GetType() == typeof(FootballEvent) || entry.Entity.GetType() == typeof(FootballEventTag) || entry.Entity.GetType() == typeof(Bet))
                        {
                            entry.State = EntityState.Detached;
                        }
                        break;
                }
            }
        }

        #endregion

        #region IModalDialogViewModel

        public bool? DialogResult { get; set; } = false;

        #endregion

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errorsViewModel.HasErrors;

        public bool CanCreate => HasErrors == false;

        public IEnumerable GetErrors(string? propertyName)
        {
            return _errorsViewModel.GetErrors(propertyName);
        }

        #endregion
    }
}
