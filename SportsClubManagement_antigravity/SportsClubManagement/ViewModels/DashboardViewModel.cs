using System.Collections.ObjectModel;
using System.Linq;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string _welcomeMessage;
        private int _totalTeams;
        private int _upcomingSessions;
        private int _totalMembers;

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public int TotalTeams
        {
            get => _totalTeams;
            set => SetProperty(ref _totalTeams, value);
        }

        public int UpcomingSessions
        {
            get => _upcomingSessions;
            set => SetProperty(ref _upcomingSessions, value);
        }

        public int TotalMembers
        {
            get => _totalMembers;
            set => SetProperty(ref _totalMembers, value);
        }

        public DashboardViewModel()
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser != null)
            {
                WelcomeMessage = $"Xin chào, {currentUser.FullName}!";

                var userTeams = DataService.Instance.TeamMembers
                    .Where(tm => tm.UserId == currentUser.Id)
                    .Select(tm => tm.TeamId)
                    .Distinct()
                    .ToList();

                TotalTeams = userTeams.Count;

                var now = DateTime.Now;
                var userSessions = DataService.Instance.Sessions
                    .Where(s => (s.UserId == currentUser.Id || (s.TeamId != null && userTeams.Contains(s.TeamId))) 
                                && s.StartTime >= now 
                                && s.StartTime.Month == now.Month 
                                && s.StartTime.Year == now.Year)
                    .Count();

                UpcomingSessions = userSessions;

                TotalMembers = DataService.Instance.TeamMembers
                    .Where(tm => userTeams.Contains(tm.TeamId))
                    .Count();
            }
        }
    }
}
