using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfLesson.Models;

namespace WpfLesson.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly PredictionService _predictionService;
        private readonly ObservableCollection<Team> _teams;
        private Team _selectedTeam;

        public MainViewModel()
        {
            _predictionService = new PredictionService();
            _teams = InitializeTeams();
            SelectTeamCommand = new RelayCommand<Team>(SelectTeam);
        }

        public ObservableCollection<Team> Teams => _teams;

        public Team SelectedTeam
        {
            get => _selectedTeam;
            set
            {
                _selectedTeam = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedTeam));
            }
        }

        public bool HasSelectedTeam => SelectedTeam != null;

        public ICommand SelectTeamCommand { get; }

        private void SelectTeam(Team team)
        {
            if (team != null)
            {
                _predictionService.PredictTeamPerformance(team);
                SelectedTeam = team;
            }
        }

        private ObservableCollection<Team> InitializeTeams()
        {
            return new ObservableCollection<Team>
            {
                new Team { Name = "KIA 타이거즈", LogoPath = "/source/kia.png" },
                new Team { Name = "삼성 라이온즈", LogoPath = "/source/samsung.png" },
                new Team { Name = "LG 트윈스", LogoPath = "/source/lg.png" },
                new Team { Name = "두산 베어스", LogoPath = "/source/doosan.png" },
                new Team { Name = "KT WIZ", LogoPath = "/source/kt.png" },
                new Team { Name = "SSG 랜더스", LogoPath = "/source/ssg.png" },
                new Team { Name = "롯데 자이언츠", LogoPath = "/source/lotte.png" },
                new Team { Name = "한화 이글스", LogoPath = "/source/hanwha.png" },
                new Team { Name = "NC 다이노스", LogoPath = "/source/nc.png" },
                new Team { Name = "키움 히어로즈", LogoPath = "/source/kiwoom.png" }
            };
        }
    }
}
