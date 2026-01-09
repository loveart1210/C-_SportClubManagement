using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamsViewModel : ViewModelBase
    {
        private ObservableCollection<TeamDisplayModel> _teams;
        private ObservableCollection<TeamDisplayModel> _allTeams;
        private TeamDisplayModel _selectedTeam;
        private string _searchText;
        private string _newTeamName = string.Empty;
        private string _newTeamDescription = string.Empty;

        public ObservableCollection<TeamDisplayModel> Teams
        {
            get => _teams;
            set => SetProperty(ref _teams, value);
        }

        public TeamDisplayModel SelectedTeam
        {
            get => _selectedTeam;
            set => SetProperty(ref _selectedTeam, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ApplyFilters();
            }
        }

        public string NewTeamName
        {
            get => _newTeamName;
            set => SetProperty(ref _newTeamName, value);
        }

        public string NewTeamDescription
        {
            get => _newTeamDescription;
            set => SetProperty(ref _newTeamDescription, value);
        }

        public ICommand ViewTeamCommand { get; }
        public ICommand CreateTeamCommand { get; }

        public TeamsViewModel()
        {
            ViewTeamCommand = new RelayCommand(ViewTeam, CanViewTeam);
            CreateTeamCommand = new RelayCommand(CreateTeam, CanCreateTeam);
            LoadTeamData();
        }

        private void LoadTeamData()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            var userTeamIds = DataService.Instance.TeamMembers
                .Where(tm => tm.UserId == currentUser.Id)
                .Select(tm => tm.TeamId)
                .ToList();

            _allTeams = new ObservableCollection<TeamDisplayModel>();

            foreach (var teamId in userTeamIds)
            {
                var team = DataService.Instance.Teams.FirstOrDefault(t => t.Id == teamId);
                var memberRole = DataService.Instance.TeamMembers
                    .FirstOrDefault(tm => tm.UserId == currentUser.Id && tm.TeamId == teamId)?.Role ?? "Member";

                var memberCount = DataService.Instance.TeamMembers
                    .Count(tm => tm.TeamId == teamId);

                if (team != null)
                {
                    _allTeams.Add(new TeamDisplayModel
                    {
                        Team = team,
                        MemberRole = memberRole,
                        MemberCount = memberCount
                    });
                }
            }

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allTeams == null) return;
            
            var filtered = _allTeams.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(t => t.Team.Name.ToLower().Contains(SearchText.ToLower()) ||
                                             t.Team.Description?.ToLower().Contains(SearchText.ToLower()) == true);
            
            Teams = new ObservableCollection<TeamDisplayModel>(filtered.ToList());
        }

        private bool CanViewTeam(object parameter)
        {
            return SelectedTeam != null;
        }

        private void ViewTeam(object parameter)
        {
            if (SelectedTeam?.Team != null)
            {
                // This would normally navigate; for now we can raise an event
                OnTeamSelected?.Invoke(this, SelectedTeam.Team);
            }
        }

        private bool CanCreateTeam(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewTeamName);
        }

        private void CreateTeam(object parameter)
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            // Create new team
            var newTeam = new Team
            {
                Name = NewTeamName,
                Description = NewTeamDescription,
                CreatedDate = DateTime.Now,
                Balance = 0
            };
            DataService.Instance.Teams.Add(newTeam);

            // Add current user as Founder
            var teamMember = new TeamMember
            {
                UserId = currentUser.Id,
                TeamId = newTeam.Id,
                Role = "Founder",
                JoinDate = DateTime.Now
            };
            DataService.Instance.TeamMembers.Add(teamMember);

            // Save changes
            DataService.Instance.Save();

            // Clear form
            NewTeamName = string.Empty;
            NewTeamDescription = string.Empty;

            // Reload teams
            LoadTeamData();

            System.Windows.MessageBox.Show($"Team '{newTeam.Name}' đã được tạo thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public event EventHandler<Team> OnTeamSelected;
    }

    public class TeamDisplayModel
    {
        public Team Team { get; set; }
        public string MemberRole { get; set; }
        public int MemberCount { get; set; }
    }
}
