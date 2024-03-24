using Beny.Commands;
using Beny.Models;
using Beny.Models.Enums;
using Beny.Repositories;
using Beny.ViewModels.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Beny.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private MainRepository mainRepository;
        public ObservableCollection<Bet> Bets { get; set; }
        public DateTime StartRange { get; set; } = DateTime.Now.AddDays(-4);
        public DateTime EndRange { get; set; } = DateTime.Now.AddDays(4);
        public RelayCommand LoadedWindow
        {
            get
            {
                return new RelayCommand(_ =>
                {
                    mainRepository.Database.EnsureCreated();

                    mainRepository.Bets.Load();
                    mainRepository.FootballEvents.Load();
                    mainRepository.Teams.Load();

                    Bets = mainRepository.Bets.Local.ToObservableCollection();

                    OnPropertyChanged(nameof(Bets));
                });
            }
        }

        public RelayCommand UpdateTableWithRange
        {
            get
            {
                return new RelayCommand(_ =>
                {
                    CollectionView collectionView = (CollectionView) CollectionViewSource.GetDefaultView(Bets);

                    collectionView.Filter = (x) =>
                    {
                        Bet bet = (Bet) x;

                        return StartRange < bet.CreatedAt && bet.CreatedAt < EndRange; 
                    };
                });
            }
        }

        public MainViewModel(MainRepository mainRepository)
        {
            this.mainRepository = mainRepository;
        }
    }
}
