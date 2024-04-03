using Beny.Commands;
using Beny.Models;
using Beny.Repositories;
using Beny.ViewModels.Base;
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
    public class BetViewModel : BindableBase, INotifyDataErrorInfo
    {
        private readonly MainRepository _mainRepository;
        private readonly ErrorsViewModel _errorsViewModel;

        private string _HomeTeam;
        private string _GuestTeam;
        private string _Competition;
        private string _Forecast;
        private string _Sport;
        private string _Coefficient;

        public ObservableCollection<string> TeamList { get; set; }
        public ObservableCollection<string> ForecastList { get; set; }
        public ObservableCollection<string> SportList { get; set; }
        public ObservableCollection<string> CompetitionList { get; set; }

        public int[] AllMinutes { get; set; }
        public int[] AllHours { get; set; }

        public int UpdateBetId { get; set; } = -1;
        public Bet CurrentBet { get; set; }


        private FootballEvent _SelectedFootballEvent;
        public FootballEvent SelectedFootballEvent {
            get
            {
                return _SelectedFootballEvent;
            }
            set
            {
                _SelectedFootballEvent = value;

                this.Sport = value?.Sport?.Name;
                this.Coefficient = value?.Coefficient.ToString();
                this.Date = value?.StartedAt.Date ?? DateTime.Now;
                this.Hour = value?.StartedAt.Hour ?? 0;
                this.Minute = value?.StartedAt.Minute ?? 0;
                this.HomeTeam = value?.HomeTeam?.Name;
                this.GuestTeam = value?.GuestTeam?.Name;
                this.Competition = value?.Competition?.Name;
                this.Forecast = value?.Forecast?.Name;

                OnPropertyChanged(nameof(SelectedFootballEvent));
            }
        }

        public string HomeTeam
        {
            get
            {
                return _HomeTeam;
            }
            set
            {
                _HomeTeam = value;
                OnPropertyChanged(nameof(HomeTeam));
            }
        }

        public string GuestTeam
        {
            get
            {
                return _GuestTeam;
            }
            set
            {
                _GuestTeam = value;
                OnPropertyChanged(nameof(GuestTeam));
            }
        }

        public string Competition
        {
            get
            {
                return _Competition;
            }
            set
            {
                _Competition = value;
                OnPropertyChanged(nameof(Competition));
            }
        }

        public string Coefficient
        {
            get
            {
                return _Coefficient;
            }
            set
            {
                _Coefficient = value;

                _errorsViewModel.ClearErrors(nameof(Coefficient));

                if(float.TryParse(_Coefficient, out float _) == false)
                {
                    _errorsViewModel.AddError(nameof(Coefficient), "Invalid Coefficient");
                }

                OnPropertyChanged(nameof(Coefficient));
            }
        }

        public string Forecast
        {
            get
            {
                return _Forecast;
            }
            set
            {
                _Forecast = value;
                OnPropertyChanged(nameof(Forecast));
            }
        }

        public string Sport
        {
            get
            {
                return _Sport;
            }
            set
            {
                _Sport = value;
                OnPropertyChanged(nameof(Sport));
            }
        }

        private DateTime _Date = DateTime.Now;
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        private int _Hour;
        public int Hour
        {
            get
            {
                return _Hour;
            }
            set
            {
                _Hour = value;
                OnPropertyChanged(nameof(Hour));
            }
        }

        private int _Minute;

        public int Minute
        {
            get
            {
                return _Minute;
            }
            set
            {
                _Minute = value;
                OnPropertyChanged(nameof(Minute));
            }
        }

        public BetViewModel(MainRepository mainRepository, ErrorsViewModel errorsViewModel)
        {
            _mainRepository = mainRepository;

            _errorsViewModel = errorsViewModel;
            _errorsViewModel.ErrorsChanged += ErrorsChanged;

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
            footballEvent.FootballEventStatus = Models.Enums.FootballEventStatus.NotCalculated;
            footballEvent.Competition = competition;
            footballEvent.Sport = sport;
            footballEvent.Forecast = forecast;
            footballEvent.HomeTeam = homeTeam;
            footballEvent.GuestTeam = guestTeam;

            if (addFootballEvent)
            {
                footballEvent.Bet = CurrentBet;
                _mainRepository.FootballEvents.Local.Add(footballEvent);
            }

            OnPropertyChanged(nameof(CurrentBet));
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

        public ICommand CreateFootballEventCommand { get; set; }
        public ICommand EditFootballEventCommand { get; set; }
        public ICommand DeleteFootballEventCommand { get; set; }
        public ICommand ClearFootballEventCommand { get; set; }
        public ICommand ClosedWindowCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand SaveBetCommand { get; set; }

        private void SaveBet(object x)
        {
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

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errorsViewModel.HasErrors;
        public bool CanCreate => HasErrors == false;

        public IEnumerable GetErrors(string? propertyName)
        {
            return _errorsViewModel.GetErrors(propertyName);
        }
    }
}
