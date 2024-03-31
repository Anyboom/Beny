using Beny.Models.Enums;
using Beny.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Models
{
    public class FootballEvent : BindableBase
    {
        public int Id { get; set; }
        public Bet Bet { get; set; }
        private Team _HomeTeam { get; set; }
        public Team HomeTeam {
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
        private Team _GuestTeam { get; set; }
        public Team GuestTeam {
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
        private Competition _Competition { get; set; }
        public Competition Competition
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
        private float _Coefficient { get; set; }
        public float Coefficient
        {
            get
            {
                return _Coefficient;
            }
            set
            {
                _Coefficient = value;
                OnPropertyChanged(nameof(Coefficient));
            }
        }
        private DateTime _StartedAt { get; set; }
        public DateTime StartedAt
        {
            get
            {
                return _StartedAt;
            }
            set
            {
                _StartedAt = value;
                OnPropertyChanged(nameof(StartedAt));
            }
        }
        private DateTime _CreatedAt { get; set; }
        public DateTime CreatedAt
        {
            get
            {
                return _CreatedAt;
            }
            set
            {
                _CreatedAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }
        private Forecast _Forecast { get; set; }
        public Forecast Forecast
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
        private FootballEventStatus _FootballEventStatus { get; set; }
        public FootballEventStatus FootballEventStatus
        {
            get
            {
                return _FootballEventStatus;
            }
            set
            {
                _FootballEventStatus = value;
                OnPropertyChanged(nameof(FootballEventStatus));
            }
        }

        private Sport _Sport { get; set; }
        public Sport Sport
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
    }
}
