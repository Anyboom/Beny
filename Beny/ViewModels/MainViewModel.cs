using Beny.Commands;
using Beny.Models;
using Beny.Models.Enums;
using Beny.Repositories;
using Beny.Services;
using Beny.Services.Interfaces;
using Beny.ViewModels.Base;
using Beny.Views.Dialogs;
using Microsoft.EntityFrameworkCore;
using SimpleInjector;
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
    public class MainViewModel : BindableBase
    {
        private MainRepository mainRepository;
        private IDialogService dialogService;

        public ObservableCollection<Bet> Bets { get; set; }
        public DateTime StartRange { get; set; } = DateTime.Now.AddDays(-4);
        public DateTime EndRange { get; set; } = DateTime.Now.AddDays(4);
        public Bet SelectedBet { get; set; }

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
                    mainRepository.Forecasts.Load();
                    mainRepository.Sports.Load();
                    mainRepository.Competitions.Load();

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

        public RelayCommand AddBet
        {
            get
            {
                return new RelayCommand(_ =>
                {
                    dialogService.ShowDialog<BetWindow, BetViewModel>();
                    OnPropertyChanged(nameof(Bets));
                });
            }
        }

        public RelayCommand EditBet
        {
            get
            {
                return new RelayCommand(_ =>
                {
                    dialogService.ShowDialog<BetWindow, BetViewModel>(x => x.UpdateBetId = SelectedBet.Id);
                    OnPropertyChanged(nameof(Bets));
                    
                });
            }
        }

        public MainViewModel(MainRepository mainRepository, IDialogService dialogService)
        {
            this.mainRepository = mainRepository;
            this.dialogService = dialogService;
        }
    }
}
