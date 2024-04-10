using Beny.Base;
using Beny.Commands;
using Beny.Enums;
using Beny.Models;
using Beny.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Beny.ViewModels
{
    public class CreateOrUpdateBetViewModel : BindableBase, INotifyDataErrorInfo
    {
        #region [Private variables]

        private readonly ErrorsViewModel _errorsViewModel;
        private readonly MainRepository _mainRepository;

        private string _homeTeam;
        private string _guestTeam;
        private string _competition;
        private string _forecast;
        private string _sport;
        private string _coefficient;
        private int _minute;
        private int _hour;

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

                IsValidatedForm = true;

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
                _homeTeam = value?.Trim();

                _errorsViewModel.ClearErrors(nameof(HomeTeam));

                if (string.IsNullOrWhiteSpace(_homeTeam))
                {
                    _errorsViewModel.AddError(nameof(HomeTeam), "Поле обязательно для заполнения.");
                }

                OnPropertyChanged(nameof(HomeTeam));
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
                _guestTeam = value?.Trim();

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
                _competition = value?.Trim();

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
                _coefficient = value?.Trim();

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
                _forecast = value?.Trim();

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
        public ICommand UpdateFootballEventStatusCommand { get; set; }

        #endregion

        public CreateOrUpdateBetViewModel(MainRepository mainRepository, ErrorsViewModel errorsViewModel)
        {
            _mainRepository = mainRepository;

            _errorsViewModel = errorsViewModel;

            _errorsViewModel.ErrorsChanged += (s, e) =>
            {
                ErrorsChanged?.Invoke(s, e);
            };

            mainRepository.Database.EnsureCreated();

            mainRepository.Forecasts.Load();
            mainRepository.Sports.Load();
            mainRepository.Competitions.Load();
            mainRepository.Teams.Load();
            mainRepository.FootballEvents.Load();

            ForecastList = new ObservableCollection<string>(mainRepository.Forecasts.Local.Select(x => x.Name));
            SportList = new ObservableCollection<string>(mainRepository.Sports.Local.Select(x => x.Name));
            CompetitionList = new ObservableCollection<string>(mainRepository.Competitions.Local.Select(x => x.Name));
            TeamList = new ObservableCollection<string>(mainRepository.Teams.Local.Select(x => x.Name));

            AllMinutes = Enumerable.Range(0, 60).ToArray();
            AllHours = Enumerable.Range(0, 24).ToArray();

            CreateFootballEventCommand = new RelayCommand(SaveFootballEvent, x => CanCreate);
            EditFootballEventCommand = new RelayCommand(SaveFootballEvent, x => CanCreate);
            ClearFootballEventCommand = new RelayCommand(ClearFootballEvent);
            DeleteFootballEventCommand = new RelayCommand(DeleteFootballEvent);
            LoadedWindowCommand = new RelayCommand(LoadedWindow);
            ClosedWindowCommand = new RelayCommand(ClosedWindow);
            SaveBetCommand = new RelayCommand(SaveBet);
            UpdateFootballEventStatusCommand = new RelayCommand(UpdateFootballEventStatus);

            HomeTeam = "Ливерпуль";
            GuestTeam = "Манчестер Сити";
            Forecast = "ТБ 2.5";
            Competition = "Англия. Премьер-лига";
            Coefficient = "1,78";
            Sport = "Футбол";
        }

        private void SaveFootballEvent(object x)
        {
            Competition? competition = _mainRepository.Competitions.Local.SingleOrDefault(x => x.Name == Competition);

            bool updateFootballEvent = x != null,
                 addFootballEvent = x == null;

            if (competition == null)
            {
                competition = new Competition()
                {
                    Name = Competition
                };

                _mainRepository.Competitions.Local.Add(competition);
                CompetitionList.Add(competition.Name);
            }

            Sport? sport = _mainRepository.Sports.Local.SingleOrDefault(x => x.Name == Sport);

            if (sport == null)
            {
                sport = new Sport()
                {
                    Name = Sport
                };

                _mainRepository.Sports.Local.Add(sport);
                SportList.Add(sport.Name);
            }

            Forecast? forecast = _mainRepository.Forecasts.Local.SingleOrDefault(x => x.Name == Forecast);

            if (forecast == null)
            {
                forecast = new Forecast()
                {
                    Name = Forecast
                };

                _mainRepository.Forecasts.Local.Add(forecast);
                ForecastList.Add(forecast.Name);
            }

            Team? homeTeam = _mainRepository.Teams.Local.SingleOrDefault(x => x.Name == HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = new Team()
                {
                    Name = HomeTeam
                };

                _mainRepository.Teams.Local.Add(homeTeam);
                TeamList.Add(homeTeam.Name);
            }

            Team? guestTeam = _mainRepository.Teams.Local.SingleOrDefault(x => x.Name == GuestTeam);

            if (guestTeam == null)
            {
                guestTeam = new Team()
                {
                    Name = GuestTeam
                };

                _mainRepository.Teams.Local.Add(guestTeam);
                TeamList.Add(guestTeam.Name);
            }

            FootballEvent footballEvent = new FootballEvent();

            if (updateFootballEvent)
            {
                footballEvent = SelectedFootballEvent;
            }

            footballEvent.CreatedAt = DateTime.Now;
            footballEvent.StartedAt = new DateTime(Date.Year, Date.Month, Date.Day, Hour, Minute, 0);
            footballEvent.Coefficient = float.Parse(Coefficient);
            footballEvent.FootballEventStatus = FootballEventStatus.NotCalculated;
            footballEvent.Competition = competition;
            footballEvent.Sport = sport;
            footballEvent.Forecast = forecast;
            footballEvent.HomeTeam = homeTeam;
            footballEvent.GuestTeam = guestTeam;

            if (addFootballEvent)
            {
                CurrentBet.FootballEvents.Add(footballEvent);
            }

            OnPropertyChanged(nameof(CurrentBet));
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
        }

        private void DeleteFootballEvent(object x)
        {
            CurrentBet.FootballEvents.Remove(SelectedFootballEvent);
        }

        private void SaveBet(object x)
        {
            if (CurrentBet.FootballEvents.Count == 0)
            {
                return;
            }

            if (UpdateBetId == -1)
            {
                _mainRepository.Add(CurrentBet);
            }

            _mainRepository.SaveChanges();

            (x as Window).Close();
        }

        private void LoadedWindow(object x)
        {
            CurrentBet = _mainRepository.Bets.Find(UpdateBetId) ?? new Bet() { CreatedAt = DateTime.Now};

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
                        if (entry.Entity.GetType() == typeof(FootballEvent))
                        {
                            entry.State = EntityState.Detached;
                        }
                        break;
                }
            }
        }

        #region [INotifyDataErrorInfo]

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errorsViewModel.HasErrors;

        public bool CanCreate => HasErrors == false;

        public bool IsValidatedForm = false;
        public IEnumerable GetErrors(string? propertyName)
        {
            return _errorsViewModel.GetErrors(propertyName);
        }

        #endregion
    }
}
