using Beny.Commands;
using Beny.Models;
using Beny.Repositories;
using Beny.ViewModels.Base;
using Beny.ViewModels.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Beny.ViewModels
{
    public class BetViewModel : BindableBase
    {
        private readonly MainRepository mainRepository;
        public ObservableCollection<string> TeamList { get; set; }
        public ObservableCollection<string> ForecastList { get; set; }
        public Bet CurrentBet { get; set; }
        public int UpdateBetId { get; set; } = -1;
        public ObservableCollection<string> SportList { get; set; }
        public ObservableCollection<string> CompetitionList { get; set; }

        public int[] AllMinutes { get; set; }
        public int[] AllHours { get; set; }

        private FootballEvent _SelectedFootballEvent;
        public FootballEvent SelectedFootballEvent {
            get
            {
                return _SelectedFootballEvent;
            }
            set
            {
                _SelectedFootballEvent = value;

                FootballEventDTO.Sport = value?.Sport.Name;
                FootballEventDTO.Coefficient = value?.Coefficient.ToString();
                FootballEventDTO.Date = value?.StartedAt.Date ?? DateTime.Now;
                FootballEventDTO.Hour = value?.StartedAt.Hour ?? 0;
                FootballEventDTO.Minute = value?.StartedAt.Minute ?? 0;
                FootballEventDTO.HomeTeam = value?.HomeTeam.Name;
                FootballEventDTO.GuestTeam = value?.GuestTeam.Name;
                FootballEventDTO.Competition = value?.Competition.Name;
                FootballEventDTO.Forecast = value?.Forecast.Name;

                OnPropertyChanged(nameof(FootballEventDTO));
                OnPropertyChanged(nameof(SelectedFootballEvent));
            }
        }

        private FootballEventDTO _FootballEventDTO;
        public FootballEventDTO FootballEventDTO {
            get
            {
                return _FootballEventDTO;
            }
            set
            {
                _FootballEventDTO = value;
                OnPropertyChanged(nameof(FootballEventDTO));
            }
        }

        public BetViewModel(MainRepository mainRepository)
        {
            this.mainRepository = mainRepository;

            mainRepository.Database.EnsureCreated();

            FootballEventDTO = new FootballEventDTO();

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
        }
        public RelayCommand AddFootballEvent
        {
            get
            {
                return new RelayCommand(x =>
                {
                    Competition? competition = mainRepository.Competitions.Local.SingleOrDefault(x => x.Name == FootballEventDTO.Competition);

                    bool updateFootballEvent = x != null,
                         addFootballEvent = x == null;

                    if (competition == null)
                    {
                        competition = new Competition()
                        {
                            Name = FootballEventDTO.Competition
                        };

                        mainRepository.Competitions.Local.Add(competition);
                        CompetitionList.Add(competition.Name);
                    }

                    Sport? sport = mainRepository.Sports.Local.SingleOrDefault(x => x.Name == FootballEventDTO.Sport);

                    if (sport == null)
                    {
                        sport = new Sport()
                        {
                            Name = FootballEventDTO.Sport
                        };

                        mainRepository.Sports.Local.Add(sport);
                        SportList.Add(sport.Name);
                    }

                    Forecast? forecast = mainRepository.Forecasts.Local.SingleOrDefault(x => x.Name == FootballEventDTO.Forecast);

                    if (forecast == null)
                    {
                        forecast = new Forecast()
                        {
                            Name = FootballEventDTO.Forecast
                        };

                        mainRepository.Forecasts.Local.Add(forecast);
                        ForecastList.Add(forecast.Name);
                    }

                    Team? homeTeam = mainRepository.Teams.Local.SingleOrDefault(x => x.Name == FootballEventDTO.HomeTeam);

                    if (homeTeam == null)
                    {
                        homeTeam = new Team()
                        {
                            Name = FootballEventDTO.HomeTeam
                        };

                        mainRepository.Teams.Local.Add(homeTeam);
                        TeamList.Add(homeTeam.Name);
                    }

                    Team? guestTeam = mainRepository.Teams.Local.SingleOrDefault(x => x.Name == FootballEventDTO.GuestTeam);

                    if (guestTeam == null)
                    {
                        guestTeam = new Team()
                        {
                            Name = FootballEventDTO.GuestTeam
                        };

                        mainRepository.Teams.Local.Add(guestTeam);
                        TeamList.Add(guestTeam.Name);
                    }

                    FootballEvent footballEvent = new FootballEvent();

                    if (updateFootballEvent)
                    {
                        footballEvent = SelectedFootballEvent;
                    }

                    footballEvent.CreatedAt = DateTime.Now;
                    footballEvent.StartedAt = new DateTime(FootballEventDTO.Date.Year, FootballEventDTO.Date.Month, FootballEventDTO.Date.Day, FootballEventDTO.Hour, FootballEventDTO.Minute, 0);
                    footballEvent.Coefficient = float.Parse(FootballEventDTO.Coefficient);
                    footballEvent.FootballEventStatus = Models.Enums.FootballEventStatus.NotCalculated;
                    footballEvent.Competition = competition;
                    footballEvent.Sport = sport;
                    footballEvent.Forecast = forecast;
                    footballEvent.HomeTeam = homeTeam;
                    footballEvent.GuestTeam = guestTeam;

                    if(addFootballEvent)
                    {
                        footballEvent.Bet = CurrentBet;
                        mainRepository.FootballEvents.Local.Add(footballEvent);
                    }

                    OnPropertyChanged(nameof(CurrentBet));
                });
            }
        }
        
        public RelayCommand SaveBet
        {
            get
            {
                return new RelayCommand((x) =>
                {
                    mainRepository.SaveChanges();

                    (x as Window).Close();
                });
            }
        }

        public RelayCommand ClearFootballEvent
        {
            get
            {
                return new RelayCommand(x => FootballEventDTO = new FootballEventDTO());
            }
        }

        public RelayCommand DeleteFootballEvent
        {
            get
            {
                return new RelayCommand(x =>
                {
                    CurrentBet.FootballEvents.Remove(SelectedFootballEvent);
                });
            }
        }

        public RelayCommand LoadedWindow
        {
            get
            {
                return new RelayCommand(x =>
                {
                    CurrentBet = mainRepository.Bets.Find(UpdateBetId) ?? new Bet();

                    OnPropertyChanged(nameof(CurrentBet));
                });
            }
        }

        public RelayCommand ClosedWindow
        {
            get
            {
                return new RelayCommand(x =>
                {
                    foreach (var entry in mainRepository.ChangeTracker.Entries())
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
                                if(entry.Entity.GetType() == typeof(FootballEvent))
                                {
                                    entry.State = EntityState.Detached;
                                }
                                break;
                        }
                    }
                });
            }
        }
    }
}
