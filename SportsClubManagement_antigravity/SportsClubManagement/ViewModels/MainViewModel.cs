using System;
using System.Windows;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentView;
        private string _userName;
        private string _userRole;
        private string _userAvatar;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        public string UserAvatar
        {
            get => _userAvatar;
            set => SetProperty(ref _userAvatar, value);
        }

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToTeamsCommand { get; }
        public ICommand NavigateToPersonalScheduleCommand { get; }
        public ICommand NavigateToAdminCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser != null)
            {
                UserName = currentUser.FullName;
                UserRole = currentUser.Role;
                UserAvatar = currentUser.AvatarPath ?? "https://via.placeholder.com/40";
            }

            NavigateToDashboardCommand = new RelayCommand(o => CurrentView = new DashboardViewModel());
            NavigateToTeamsCommand = new RelayCommand(o => CurrentView = new TeamsViewModel());
            NavigateToPersonalScheduleCommand = new RelayCommand(o => {
                var vm = new PersonalScheduleViewModel();
                if (o != null && int.TryParse(o.ToString(), out int index))
                {
                    vm.SelectedTabIndex = index;
                }
                CurrentView = vm;
            });
            NavigateToProfileCommand = new RelayCommand(o => CurrentView = new ProfileViewModel());
            NavigateToAdminCommand = new RelayCommand(o => CurrentView = new UserManagementViewModel());
            LogoutCommand = new RelayCommand(ExecuteLogout);

            // Default View
            CurrentView = new DashboardViewModel();
        }

        private void ExecuteLogout(object obj)
        {
            DataService.Instance.CurrentUser = null;
            Application.Current.Shutdown();
        }
    }
}
