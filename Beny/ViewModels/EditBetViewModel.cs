﻿using Beny.Base;
using Beny.Commands;
using Beny.Enums;
using Beny.Models;
using Beny.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Beny.Collections;
using Beny.Views;
using MvvmDialogs;

namespace Beny.ViewModels
{
    public class EditBetViewModel : BindableBase, INotifyDataErrorInfo, IModalDialogViewModel
    {
        #region [Private variables]

        private readonly ErrorsViewModel _errorsViewModel;
        private readonly MainRepository _mainRepository;
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

        #region [Getters/Setters]

        public ObservableCollection<string> TeamList { get; set; }
        public ObservableCollection<string> ForecastList { get; set; }
        public ObservableCollection<string> SportList { get; set; }
        public ObservableCollection<string> CompetitionList { get; set; }

        public int[] AllMinutes { get; set; }
        public int[] AllHours { get; set; }

        public int UpdateBetId { get; set; } = -1;
        public Bet CurrentBet { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }

        public string Title
        {
            get
            {
                return (UpdateBetId == -1) ? "Бени - создание ставки" : "Бени - редактирование ставки";
            }
        }

        public FootballEvent SelectedFootballEvent
        {
            get
            {
                return _selectedFootballEvent;
            }
            set
            {
                _selectedFootballEvent = value;

                this.Sport = value?.Sport?.Name;
                this.Coefficient = value?.Coefficient.ToString();
                this.Date = value?.StartedAt.Date ?? DateTime.Now;
                this.Hour = value?.StartedAt.Hour ?? 0;
                this.Minute = value?.StartedAt.Minute ?? 0;
                this.HomeTeam = value?.HomeTeam?.Name;
                this.GuestTeam = value?.GuestTeam?.Name;
                this.Competition = value?.Competition?.Name;
                this.Forecast = value?.Forecast?.Name;

                this.Tags = new ObservableCollection<Tag>(value?.FootballEventTags.Select(x => x.Tag));

                this.IsLive = (bool) value?.IsLive;

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
                _sport = value?.Trim();

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

        #region [Commands]

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

        #region CreateOrUpdateBetViewModel

        public EditBetViewModel(MainRepository mainRepository, IDialogService dialogService, SelectionViewModel<Tag> selectionViewModel)
        {
            _mainRepository = mainRepository;
            _dialogService = dialogService;
            _selectionViewModel = selectionViewModel;

            _errorsViewModel = new ErrorsViewModel();

            _errorsViewModel.ErrorsChanged += (s, e) =>
            {
                ErrorsChanged?.Invoke(s, e);
            };

            ForecastList = new ObservableCollection<string>(_mainRepository.Forecasts.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));
            SportList = new ObservableCollection<string>(_mainRepository.Sports.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));
            CompetitionList = new ObservableCollection<string>(_mainRepository.Competitions.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));
            TeamList = new ObservableCollection<string>(_mainRepository.Teams.Local.Where(x => x.DeletedAt == null).Select(x => x.Name));

            AllMinutes = Enumerable.Range(0, 60).ToArray();
            AllHours = Enumerable.Range(0, 24).ToArray();

            CreateFootballEventCommand = new RelayCommand(SaveFootballEvent, x => CanCreate);
            EditFootballEventCommand = new RelayCommand(SaveFootballEvent, x => CanCreate && SelectedFootballEvent != null);
            ClearFootballEventCommand = new RelayCommand(ClearFootballEvent);
            DeleteFootballEventCommand = new RelayCommand(DeleteFootballEvent, x => SelectedFootballEvent != null);

            LoadedWindowCommand = new RelayCommand(LoadedWindow);
            ClosedWindowCommand = new RelayCommand(ClosedWindow);

            SaveBetCommand = new RelayCommand(SaveBet, x => CurrentBet?.FootballEvents.Count > 0);

            UpdateFootballEventStatusCommand = new RelayCommand(UpdateFootballEventStatus, x => SelectedFootballEvent != null);

            OpenTagsWindowCommand = new RelayCommand(OpenTagsWindow);

            
        }

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

        private void SaveFootballEvent(object x)
        {
            bool updateFootballEvent = x != null,
                 addFootballEvent = x == null;

            Competition? competition = _mainRepository.Competitions.Local.SingleOrDefault(x => x.Name == Competition);

            if (competition == null)
            {
                competition = new Competition()
                {
                    Name = Competition.Trim()
                };

                _mainRepository.Competitions.Local.Add(competition);

                CompetitionList.Add(competition.Name);
            }
            else
            {
                competition.DeletedAt = null;

                CompetitionList.Add(competition.Name);
            }

            Sport? sport = _mainRepository.Sports.Local.SingleOrDefault(x => x.Name == Sport);

            if (sport == null)
            {
                sport = new Sport()
                {
                    Name = Sport.Trim()
                };

                _mainRepository.Sports.Local.Add(sport);

                SportList.Add(sport.Name);
            }
            else
            {
                sport.DeletedAt = null;

                SportList.Add(sport.Name);
            }

            Forecast? forecast = _mainRepository.Forecasts.Local.SingleOrDefault(x => x.Name == Forecast);

            if (forecast == null)
            {
                forecast = new Forecast()
                {
                    Name = Forecast.Trim()
                };

                _mainRepository.Forecasts.Local.Add(forecast);

                ForecastList.Add(forecast.Name);
            }
            else
            {
                forecast.DeletedAt = null;

                ForecastList.Add(forecast.Name);
            }

            Team? homeTeam = _mainRepository.Teams.Local.SingleOrDefault(x => x.Name == HomeTeam.Trim());

            if (homeTeam == null)
            {
                homeTeam = new Team()
                {
                    Name = HomeTeam.Trim()
                };

                _mainRepository.Teams.Local.Add(homeTeam);

                TeamList.Add(homeTeam.Name);
            }
            else
            {
                homeTeam.DeletedAt = null;

                TeamList.Add(homeTeam.Name);
            }

            Team? guestTeam = _mainRepository.Teams.Local.SingleOrDefault(x => x.Name == GuestTeam.Trim());

            if (guestTeam == null)
            {
                guestTeam = new Team()
                {
                    Name = GuestTeam.Trim()
                };

                _mainRepository.Teams.Local.Add(guestTeam);
                TeamList.Add(guestTeam.Name);
            }
            else
            {
                guestTeam.DeletedAt = null;

                TeamList.Add(guestTeam.Name);
            }

            FootballEvent footballEvent = new FootballEvent();

            if (updateFootballEvent)
            {
                footballEvent = SelectedFootballEvent;
            }

            footballEvent.StartedAt = new DateTime(Date.Year, Date.Month, Date.Day, Hour, Minute, 0);
            footballEvent.Coefficient = float.Parse(Coefficient);
            footballEvent.FootballEventStatus = FootballEventStatus.NotCalculated;
            footballEvent.Competition = competition;
            footballEvent.Sport = sport;
            footballEvent.Forecast = forecast;
            footballEvent.HomeTeam = homeTeam;
            footballEvent.GuestTeam = guestTeam;
            footballEvent.IsLive = IsLive;

            footballEvent.FootballEventTags.Clear();

            foreach (Tag tag in Tags)
            {
                footballEvent.FootballEventTags.Add(new FootballEventTag() { Tag = tag });
            }

            if (addFootballEvent)
            {
                footballEvent.CreatedAt = DateTime.Now;
                CurrentBet.FootballEvents.Add(footballEvent);
            }

            OnPropertyChanged(nameof(CurrentBet));
            OnPropertyChanged(nameof(SelectedFootballEvent));
        }

        private void UpdateFootballEventStatus(object x)
        {
            FootballEventStatus updateStatus = (FootballEventStatus) x;

            SelectedFootballEvent.FootballEventStatus = updateStatus;
        }

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

        private void DeleteFootballEvent(object x)
        {
            CurrentBet.FootballEvents.Remove(SelectedFootballEvent);
        }

        private void SaveBet(object x)
        {
            if (UpdateBetId == -1)
            {
                _mainRepository.Add(CurrentBet);
            }

            _mainRepository.SaveChanges();

            DialogResult = true;

            _dialogService.Close(this);
        }

        private void LoadedWindow(object x)
        {
            CurrentBet = _mainRepository.Bets.Local.FirstOrDefault(x => x.Id == UpdateBetId, new Bet() { CreatedAt = DateTime.Now, FootballEvents = new ItemObservableCollection<FootballEvent>()});

            HomeTeam = "Ливерпуль";
            GuestTeam = "Манчестер Сити";
            Forecast = "ТБ 2.5";
            Competition = "Англия. Премьер-лига";
            Coefficient = "1,78";
            Sport = "Футбол";
            Tags = new ObservableCollection<Tag>();
            Date = DateTime.Now;

            _selectedFootballEvent = default;

            OnPropertyChanged(nameof(SelectedFootballEvent));
            OnPropertyChanged(nameof(CurrentBet));
        }

        private void ClosedWindow(object x)
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
                        if (entry.Entity.GetType() == typeof(FootballEvent) || entry.Entity.GetType() == typeof(FootballEventTag))
                        {
                            entry.State = EntityState.Detached;
                        }
                        break;
                }
            }
        }

        #endregion

        #region [IModalDialogViewModel]

        public bool? DialogResult { get; set; } = false;

        #endregion

        #region [INotifyDataErrorInfo]

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
