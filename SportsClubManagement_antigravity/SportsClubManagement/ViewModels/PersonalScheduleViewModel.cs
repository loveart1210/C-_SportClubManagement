using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class PersonalScheduleViewModel : ViewModelBase
    {
        private User _currentUser;
        private ObservableCollection<Subject> _subjects;
        private ObservableCollection<Session> _sessions;
        private DateTime _selectedDate;
        private int _selectedTabIndex;
        
        // Subject creation/editing
        private string _newSubjectName = string.Empty;
        private string _newSubjectDesc = string.Empty;
        private Subject _editingSubject = null; // Track if editing

        // Session creation
        private string _newSessionName = string.Empty;
        private Subject _selectedSubject;
        private string _newSessionStart = string.Empty;
        private string _newSessionEnd = string.Empty;
        private string _newSessionNote = string.Empty;
        
        // Filtering
        private string _filterStatus = "Tất cả";
        private string _filterDay = "Tất cả";
        private string _filterMonth = "Tất cả";
        private string _filterYear = "Tất cả";
        private string _filterSessionName = "Tất cả";
        private string _filterSubjectName = "Tất cả";
        private string _filterStartTime = "Tất cả";
        private string _filterEndTime = "Tất cả";
        
        private ObservableCollection<string> _dayOptions;
        private ObservableCollection<string> _monthOptions;
        private ObservableCollection<string> _yearOptions;
        private ObservableCollection<string> _sessionNameOptions;
        private ObservableCollection<string> _subjectNameOptions;
        private ObservableCollection<string> _startTimeOptions;
        private ObservableCollection<string> _endTimeOptions;

        private ObservableCollection<SessionViewItem> _sessionViewItems;


        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        public ObservableCollection<SessionViewItem> SessionViewItems
        {
            get => _sessionViewItems;
            set => SetProperty(ref _sessionViewItems, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    LoadSessions();
                }
            }
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }
        public string NewSubjectName
        {
            get => _newSubjectName;
            set => SetProperty(ref _newSubjectName, value);
        }

        public string NewSubjectDesc
        {
            get => _newSubjectDesc;
            set => SetProperty(ref _newSubjectDesc, value);
        }

        public string NewSessionName
        {
            get => _newSessionName;
            set => SetProperty(ref _newSessionName, value);
        }

        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public string NewSessionStart
        {
            get => _newSessionStart;
            set => SetProperty(ref _newSessionStart, value);
        }

        public string NewSessionEnd
        {
            get => _newSessionEnd;
            set => SetProperty(ref _newSessionEnd, value);
        }

        public string NewSessionNote
        {
            get => _newSessionNote;
            set => SetProperty(ref _newSessionNote, value);
        }

        // Filter Properties
        public string FilterStatus { get => _filterStatus; set { if (SetProperty(ref _filterStatus, value)) LoadSessions(); } }
        public string FilterDay { get => _filterDay; set { if (SetProperty(ref _filterDay, value)) LoadSessions(); } }
        public string FilterMonth { get => _filterMonth; set { if (SetProperty(ref _filterMonth, value)) LoadSessions(); } }
        public string FilterYear { get => _filterYear; set { if (SetProperty(ref _filterYear, value)) LoadSessions(); } }
        public string FilterSessionName { get => _filterSessionName; set { if (SetProperty(ref _filterSessionName, value)) LoadSessions(); } }
        public string FilterSubjectName { get => _filterSubjectName; set { if (SetProperty(ref _filterSubjectName, value)) LoadSessions(); } }
        public string FilterStartTime { get => _filterStartTime; set { if (SetProperty(ref _filterStartTime, value)) LoadSessions(); } }
        public string FilterEndTime { get => _filterEndTime; set { if (SetProperty(ref _filterEndTime, value)) LoadSessions(); } }

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "Tất cả", "Chưa bắt đầu", "Hoàn thành", "Quá hạn" };
        public ObservableCollection<string> DayOptions { get => _dayOptions; set => SetProperty(ref _dayOptions, value); }
        public ObservableCollection<string> MonthOptions { get => _monthOptions; set => SetProperty(ref _monthOptions, value); }
        public ObservableCollection<string> YearOptions { get => _yearOptions; set => SetProperty(ref _yearOptions, value); }
        public ObservableCollection<string> SessionNameOptions { get => _sessionNameOptions; set => SetProperty(ref _sessionNameOptions, value); }
        public ObservableCollection<string> SubjectNameOptions { get => _subjectNameOptions; set => SetProperty(ref _subjectNameOptions, value); }
        public ObservableCollection<string> StartTimeOptions { get => _startTimeOptions; set => SetProperty(ref _startTimeOptions, value); }
        public ObservableCollection<string> EndTimeOptions { get => _endTimeOptions; set => SetProperty(ref _endTimeOptions, value); }


        public ICommand AddSubjectCommand { get; }
        public ICommand EditSubjectCommand { get; }
        public ICommand RemoveSubjectCommand { get; }
        public ICommand AddSessionCommand { get; }
        public ICommand RemoveSessionCommand { get; }
        public ICommand ExportScheduleCommand { get; }
        public ICommand AttendSessionCommand { get; }
        public ICommand ClearFiltersCommand { get; }


        public PersonalScheduleViewModel()
        {
            _currentUser = DataService.Instance.CurrentUser;
            _selectedDate = DateTime.Today;
            _newSessionStart = DateTime.Now.ToString("HH:mm");
            _newSessionEnd = DateTime.Now.AddHours(1).ToString("HH:mm");

            if (_currentUser != null)
            {
                LoadSubjects();
                InitializeFilterOptions();
                LoadSessions();
            }

            AddSubjectCommand = new RelayCommand(AddSubject, CanAddSubject);
            EditSubjectCommand = new RelayCommand(EditSubject);
            RemoveSubjectCommand = new RelayCommand(RemoveSubject);
            AddSessionCommand = new RelayCommand(AddSession, CanAddSession);
            RemoveSessionCommand = new RelayCommand(RemoveSession);
            ExportScheduleCommand = new RelayCommand(ExportSchedule);
            AttendSessionCommand = new RelayCommand(AttendSession);
            ClearFiltersCommand = new RelayCommand(o => ClearFilters());
        }


        private void LoadSubjects()
        {
            var list = DataService.Instance.Subjects
                .Where(s => s.UserId == _currentUser.Id) // Filter by User
                .ToList();
            Subjects = new ObservableCollection<Subject>(list);
            
            SubjectNameOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(list.Select(s => s.Name).Distinct()).ToList());
            
            if (SelectedSubject == null) 
                SelectedSubject = Subjects.FirstOrDefault();
        }

        private void InitializeFilterOptions()
        {
            DayOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(Enumerable.Range(1, 31).Select(i => i.ToString())).ToList());
            MonthOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(Enumerable.Range(1, 12).Select(i => i.ToString())).ToList());
            YearOptions = new ObservableCollection<string>(new[] { "Tất cả", "2024", "2025", "2026" });
            
            UpdateDynamicFilterOptions();
        }

        private void UpdateDynamicFilterOptions()
        {
            var userSessions = DataService.Instance.Sessions.Where(s => s.UserId == _currentUser.Id).ToList();
            
            SessionNameOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(userSessions.Select(s => s.Name).Distinct()).ToList());
            StartTimeOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(userSessions.Select(s => s.StartTime.ToString("HH:mm")).Distinct().OrderBy(t => t)).ToList());
            EndTimeOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(userSessions.Select(s => s.EndTime.ToString("HH:mm")).Distinct().OrderBy(t => t)).ToList());
        }

        private void LoadSessions()
        {
            var now = DateTime.Now;
            var allSessions = DataService.Instance.Sessions
                .Where(s => s.UserId == _currentUser.Id)
                .ToList();

            var filtered = allSessions.AsQueryable();

            // Default view: if all filters are "Tất cả", show upcoming and in SELECTED MONTH
            bool isFiltering = FilterStatus != "Tất cả" || FilterDay != "Tất cả" || FilterMonth != "Tất cả" || 
                               FilterYear != "Tất cả" || FilterSessionName != "Tất cả" ||
                               FilterSubjectName != "Tất cả" || FilterStartTime != "Tất cả" ||
                               FilterEndTime != "Tất cả";

            if (!isFiltering)
            {
                // Show upcoming sessions in the month of SelectedDate
                filtered = filtered.Where(s => s.StartTime >= now && s.StartTime.Month == SelectedDate.Month && s.StartTime.Year == SelectedDate.Year);
            }
            else
            {
                // Apply filters
                if (FilterStatus != "Tất cả")
                {
                    if (FilterStatus == "Hoàn thành") filtered = filtered.Where(s => s.IsAttended);
                    else if (FilterStatus == "Quá hạn") filtered = filtered.Where(s => !s.IsAttended && s.EndTime < now);
                    else if (FilterStatus == "Chưa bắt đầu") filtered = filtered.Where(s => !s.IsAttended && s.StartTime > now);
                }

                if (FilterDay != "Tất cả" && int.TryParse(FilterDay, out int d)) filtered = filtered.Where(s => s.StartTime.Day == d);
                if (FilterMonth != "Tất cả" && int.TryParse(FilterMonth, out int m)) filtered = filtered.Where(s => s.StartTime.Month == m);
                if (FilterYear != "Tất cả" && int.TryParse(FilterYear, out int y)) filtered = filtered.Where(s => s.StartTime.Year == y);
                
                if (FilterSessionName != "Tất cả")
                    filtered = filtered.Where(s => s.Name == FilterSessionName);
                
                if (FilterSubjectName != "Tất cả")
                {
                    var subjectsDict = DataService.Instance.Subjects.ToDictionary(sub => sub.Id, sub => sub.Name);
                    filtered = filtered.Where(s => subjectsDict.ContainsKey(s.SubjectId) && subjectsDict[s.SubjectId] == FilterSubjectName);
                }

                if (FilterStartTime != "Tất cả")
                    filtered = filtered.Where(s => s.StartTime.ToString("HH:mm") == FilterStartTime);

                if (FilterEndTime != "Tất cả")
                    filtered = filtered.Where(s => s.EndTime.ToString("HH:mm") == FilterEndTime);
            }

            var result = filtered.OrderBy(s => s.StartTime).ToList();
            var subjects = DataService.Instance.Subjects.ToDictionary(s => s.Id, s => s.Name);
            
            SessionViewItems = new ObservableCollection<SessionViewItem>(
                result.Select(s => new SessionViewItem(s, subjects.ContainsKey(s.SubjectId) ? subjects[s.SubjectId] : "Unknown"))
            );
        }

        private void AttendSession(object obj)
        {
            if (obj is SessionViewItem item)
            {
                var session = DataService.Instance.Sessions.FirstOrDefault(s => s.Id == item.Session.Id);
                if (session != null)
                {
                    session.IsAttended = true;
                    DataService.Instance.Save();
                    LoadSessions();
                }
            }
        }

        private void ClearFilters()
        {
            _filterStatus = "Tất cả";
            _filterDay = "Tất cả";
            _filterMonth = "Tất cả";
            _filterYear = "Tất cả";
            _filterSessionName = "Tất cả";
            _filterSubjectName = "Tất cả";
            _filterStartTime = "Tất cả";
            _filterEndTime = "Tất cả";
            
            OnPropertyChanged(nameof(FilterStatus));
            OnPropertyChanged(nameof(FilterDay));
            OnPropertyChanged(nameof(FilterMonth));
            OnPropertyChanged(nameof(FilterYear));
            OnPropertyChanged(nameof(FilterSessionName));
            OnPropertyChanged(nameof(FilterSubjectName));
            OnPropertyChanged(nameof(FilterStartTime));
            OnPropertyChanged(nameof(FilterEndTime));
            
            LoadSessions();
        }

        private bool CanAddSubject(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewSubjectName);
        }

        private void AddSubject(object obj)
        {
            if (_editingSubject != null)
            {
                // Update existing subject by removing and re-adding
                DataService.Instance.Subjects.Remove(_editingSubject);
                var updatedSubject = new Subject
                {
                    Id = _editingSubject.Id, // Keep same ID
                    UserId = _currentUser.Id,
                    Name = NewSubjectName,
                    Description = NewSubjectDesc,
                    TeamId = null,
                    CreatedDate = _editingSubject.CreatedDate
                };
                DataService.Instance.Subjects.Add(updatedSubject);
                DataService.Instance.Save();
                _editingSubject = null;
            }
            else
            {
                // Create new subject
                var sub = new Subject
                {
                    UserId = _currentUser.Id,
                    Name = NewSubjectName,
                    Description = NewSubjectDesc,
                    TeamId = null
                };
                DataService.Instance.Subjects.Add(sub);
                DataService.Instance.Save();
            }
            
            NewSubjectName = "";
            NewSubjectDesc = "";
            LoadSubjects();
        }

        private void EditSubject(object obj)
        {
            if (obj is Subject s)
            {
                _editingSubject = s;
                NewSubjectName = s.Name;
                NewSubjectDesc = s.Description ?? "";
            }
        }

        private void RemoveSubject(object obj)
        {
            if (obj is Subject s)
            {
                DataService.Instance.Subjects.Remove(s);
                DataService.Instance.Save();
                LoadSubjects();
            }
        }

        private bool CanAddSession(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewSessionName) && SelectedSubject != null;
        }

        private void AddSession(object obj)
        {
            if (!TimeSpan.TryParse(NewSessionStart, out TimeSpan startTime) || 
                !TimeSpan.TryParse(NewSessionEnd, out TimeSpan endTime))
            {
                System.Windows.MessageBox.Show("Định dạng thời gian không hợp lệ (HH:mm).", "Lỗi", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var sess = new Session
            {
                UserId = _currentUser.Id,
                Name = NewSessionName,
                SubjectId = SelectedSubject.Id,
                StartTime = SelectedDate.Date + startTime,
                EndTime = SelectedDate.Date + endTime,
                Note = NewSessionNote,
                TeamId = null
            };
            DataService.Instance.Sessions.Add(sess);
            DataService.Instance.Save();
            UpdateDynamicFilterOptions();
            LoadSessions();
            
            // Clear inputs
            NewSessionName = "";
            NewSessionNote = "";
        }

        private void RemoveSession(object obj)
        {
            if (obj is SessionViewItem item)
            {
                DataService.Instance.Sessions.Remove(item.Session);
                DataService.Instance.Save();
                UpdateDynamicFilterOptions();
                LoadSessions();
            }
        }

        private void ExportSchedule(object obj)
        {
            if (_currentUser != null)
            {
                var filePath = ExportService.ExportPersonalScheduleToCSV(_currentUser);
                System.Windows.MessageBox.Show($"Lịch tập cá nhân đã được xuất tại:\n{filePath}", "Xuất thành công", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }

    public class SessionViewItem
    {
        public Session Session { get; }
        public string SubjectName { get; }

        public SessionViewItem(Session session, string subjectName)
        {
            Session = session;
            SubjectName = subjectName;
        }

        public string Status
        {
            get
            {
                if (Session.IsAttended) return "Hoàn thành";
                if (DateTime.Now > Session.EndTime) return "Quá hạn";
                if (DateTime.Now < Session.StartTime) return "Chưa bắt đầu";
                return "Đang diễn ra";
            }
        }

        public string AttendanceColor
        {
            get
            {
                if (Session.IsAttended) return "#FFFFFF"; // White after attendance
                if (DateTime.Now > Session.EndTime) return "#F44336"; // Red if overdue
                if (DateTime.Now >= Session.StartTime && DateTime.Now <= Session.EndTime) return "#4CAF50"; // Green if in range
                return "#FFFFFF"; // White if not started yet
            }
        }

        public bool IsAttendanceEnabled
        {
            get
            {
                return !Session.IsAttended && DateTime.Now >= Session.StartTime && DateTime.Now <= Session.EndTime;
            }
        }
    }
}
