using Beny.Commands;
using Beny.Models;
using Beny.Repositories;
using Beny.ViewModels.Base;
using Beny.ViewModels.DTO;
using Microsoft.EntityFrameworkCore;
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
    public class BetViewModel : ViewModel
    {
        private readonly MainRepository mainRepository;
        public List<string> TeamList { get; set; }
        public List<string> ForecastList { get; set; }
        public List<string> SportList { get; set; }
        public List<string> CompetitionList { get; set; }
        public ObservableCollection<FootballEvent> FootballEvents { get; set; }
        public int[] AllMinutes { get; set; }
        public int[] AllHours { get; set; }

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

            FootballEvents = new ObservableCollection<FootballEvent>();

            ForecastList = mainRepository.Forecasts.Select(x => x.Name).ToList();
            SportList = mainRepository.Sports.Select(x => x.Name).ToList();
            CompetitionList = mainRepository.Competitions.Select(x => x.Name).ToList();
            TeamList = mainRepository.Teams.Select(x => x.Name).ToList();

            AllMinutes = Enumerable.Range(0, 60).ToArray();
            AllHours = Enumerable.Range(0, 24).ToArray();
        }

        public RelayCommand AddFootballEvent
        {
            get
            {
                return new RelayCommand((x =>
                {
                    FootballEvent footballEvent = new FootballEvent();

                    footballEvent.CreatedAt = DateTime.Now;
                    footballEvent.Coefficient = float.Parse(FootballEventDTO.Coefficient);
                    footballEvent.Competition = new Competition() { Name = FootballEventDTO.Competition};
                    footballEvent.FootballEventStatus = Models.Enums.FootballEventStatus.NotCalculated;
                    footballEvent.Sport = new Sport () { Name = FootballEventDTO.Sport};
                    footballEvent.Forecast = new Forecast() {  Name = FootballEventDTO.Forecast };
                    footballEvent.HomeTeam = new Team() { Name = FootballEventDTO.HomeTeam };
                    footballEvent.GuestTeam = new Team() { Name = FootballEventDTO.GuestTeam};
                    footballEvent.StartedAt = new DateTime(FootballEventDTO.Date.Year, FootballEventDTO.Date.Month, FootballEventDTO.Date.Day, FootballEventDTO.Hour, FootballEventDTO.Minute, 0);

                    FootballEvents.Add(footballEvent);
                }));
            }
        }

        public RelayCommand ClearFootballEvent
        {
            get
            {
                return new RelayCommand((x => FootballEventDTO = new FootballEventDTO()));
            }
        }
    }
}
