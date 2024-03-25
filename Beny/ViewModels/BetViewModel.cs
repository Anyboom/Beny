using Beny.Models;
using Beny.Repositories;
using Beny.ViewModels.Base;
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
        private MainRepository mainRepository;
        public List<string> TeamList { get; set; }
        public ObservableCollection<FootballEvent> footballEvents { get; set; }
        public int[] AllMinutes { get; set; }
        public int[] AllHours { get; set; }
        public BetViewModel(MainRepository mainRepository)
        {
            this.mainRepository = mainRepository;

            mainRepository.Database.EnsureCreated();

            mainRepository.Bets.Load();
            mainRepository.FootballEvents.Load();
            mainRepository.Teams.Load();

            footballEvents = mainRepository.FootballEvents.Local.ToObservableCollection();

            TeamList = ["Ливерпуль", "Челси", "Спартак", "Зенит", "Ахмат"];

            AllMinutes = Enumerable.Range(0, 60).ToArray();
            AllHours = Enumerable.Range(0, 24).ToArray();
        }
    }
}
