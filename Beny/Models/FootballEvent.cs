﻿using Beny.Base;
using Beny.Collections;
using Beny.Models.Enums;

namespace Beny.Models
{
    public class FootballEvent : NotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        private Bet _bet;
        public Bet Bet
        {
            get
            {
                return _bet;
            }
            set
            {
                _bet = value;
                OnPropertyChanged(nameof(Bet));
            }
        }
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
        private float? _Coefficient { get; set; }
        public float? Coefficient
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

        private bool _isAlive = false;

        public bool IsLive
        {
            get
            {
                return _isAlive;
            }
            set
            {
                _isAlive = value;
                OnPropertyChanged(nameof(IsLive));
            }
        }

        public ItemObservableCollection<FootballEventTag> FootballEventTags { get; set; } = new ItemObservableCollection<FootballEventTag>();

        public FootballEvent()
        {
            FootballEventTags.CollectionChanged += (_, _) => Update();
            FootballEventTags.ItemPropertyChanged += (_, _) => Update();
        }

        private void Update()
        {
            OnPropertyChanged(nameof(FootballEventTags));
        }
    }
}
