using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using SportsClubManagement.Models;

namespace SportsClubManagement.Services
{
    public class DataService
    {
        private static readonly string FILE_NAME = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SportsClubManagement",
            "data.json"
        );
        
        private static DataService _instance;
        public static DataService Instance => _instance ??= new DataService();

        public List<User> Users { get; set; } = new List<User>();
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<Session> Sessions { get; set; } = new List<Session>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public List<FundTransaction> Transactions { get; set; } = new List<FundTransaction>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public User CurrentUser { get; set; }

        public DataService()
        {
            Load();
            if (!Users.Any()) SeedData();
            else EnsureDemoUsers();
        }

        public void Load()
        {
            try
            {
                var directory = Path.GetDirectoryName(FILE_NAME);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                if (File.Exists(FILE_NAME))
                {
                    var json = File.ReadAllText(FILE_NAME);
                    var data = JsonSerializer.Deserialize<DataWrapper>(json);
                    if (data != null)
                    {
                        Users = data.Users ?? new List<User>();
                        Teams = data.Teams ?? new List<Team>();
                        TeamMembers = data.TeamMembers ?? new List<TeamMember>();
                        Subjects = data.Subjects ?? new List<Subject>();
                        Sessions = data.Sessions ?? new List<Session>();
                        Attendances = data.Attendances ?? new List<Attendance>();
                        Transactions = data.Transactions ?? new List<FundTransaction>();
                        Notifications = data.Notifications ?? new List<Notification>();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                throw;
            }
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(FILE_NAME);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                var data = new DataWrapper
                {
                    Users = Users,
                    Teams = Teams,
                    TeamMembers = TeamMembers,
                    Subjects = Subjects,
                    Sessions = Sessions,
                    Attendances = Attendances,
                    Transactions = Transactions,
                    Notifications = Notifications
                };
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FILE_NAME, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
                throw;
            }
        }

        private void SeedData()
        {
            var admin = new User
            {
                Username = "admin",
                Password = "admin123",
                FullName = "Admin System",
                Role = "Admin",
                Email = "admin@sports.club",
                BirthDate = new DateTime(1990, 1, 1)
            };
            Users.Add(admin);

            var user1 = new User
            {
                Username = "user1",
                Password = "user123",
                FullName = "Nguyễn Văn A",
                Role = "User",
                Email = "user1@sports.club",
                BirthDate = new DateTime(2000, 5, 15)
            };
            Users.Add(user1);

            var user2 = new User
            {
                Username = "user2",
                Password = "user123",
                FullName = "Trần Văn B",
                Role = "User",
                Email = "user2@sports.club",
                BirthDate = new DateTime(2001, 3, 20)
            };
            Users.Add(user2);

            var team1 = new Team
            {
                Name = "Bóng Chuyền A",
                Description = "Đội bóng chuyền chính",
                Balance = 5000000
            };
            Teams.Add(team1);

            TeamMembers.Add(new TeamMember
            {
                TeamId = team1.Id,
                UserId = user1.Id,
                Role = "Founder",
                JoinDate = DateTime.Now
            });

            TeamMembers.Add(new TeamMember
            {
                TeamId = team1.Id,
                UserId = user2.Id,
                Role = "Coach",
                JoinDate = DateTime.Now.AddDays(-10)
            });

            var subject1 = new Subject
            {
                TeamId = team1.Id,
                Name = "Kỹ thuật cơ bản",
                Description = "Học các kỹ thuật chuyền, phát bóng",
                ParticipantCount = 2
            };
            Subjects.Add(subject1);

            var sessionDate = DateTime.Now.AddDays(2);
            var session1 = new Session
            {
                TeamId = team1.Id,
                SubjectId = subject1.Id,
                Name = "Buổi tập chiều",
                StartTime = new DateTime(sessionDate.Year, sessionDate.Month, sessionDate.Day, 15, 0, 0),
                EndTime = new DateTime(sessionDate.Year, sessionDate.Month, sessionDate.Day, 17, 0, 0),
                Note = "Mang theo nước uống"
            };
            Sessions.Add(session1);

            var notif = new Notification
            {
                TeamId = team1.Id,
                ByUserId = user1.Id,
                Title = "Thông báo tạo team",
                Content = "Team Bóng Chuyền A vừa được tạo",
                IsSystemNotification = true
            };
            Notifications.Add(notif);

            Save();
        }

        private void EnsureDemoUsers()
        {
            bool changed = false;
            
            if (!Users.Any(u => u.Username == "admin"))
            {
                Users.Add(new User
                {
                    Username = "admin",
                    Password = "admin123",
                    FullName = "Admin System",
                    Role = "Admin",
                    Email = "admin@sports.club",
                    BirthDate = new DateTime(1990, 1, 1)
                });
                changed = true;
            }

            if (!Users.Any(u => u.Username == "user1"))
            {
                Users.Add(new User
                {
                    Username = "user1",
                    Password = "user123",
                    FullName = "Nguyễn Văn A",
                    Role = "User",
                    Email = "user1@sports.club",
                    BirthDate = new DateTime(2000, 5, 15)
                });
                changed = true;
            }

            if (!Users.Any(u => u.Username == "user2"))
            {
                Users.Add(new User
                {
                    Username = "user2",
                    Password = "user123",
                    FullName = "Trần Văn B",
                    Role = "User",
                    Email = "user2@sports.club",
                    BirthDate = new DateTime(2001, 3, 20)
                });
                changed = true;
            }

            if (changed) Save();
        }

        public bool CanManageTeam(User user, Team team)
        {
            if (user.Role == "Admin") return true;
            var membership = TeamMembers.FirstOrDefault(tm => tm.UserId == user.Id && tm.TeamId == team.Id);
            return membership?.Role == "Founder" || membership?.Role == "Admin";
        }

        public bool CanManageAttendance(User user, Team team)
        {
            if (user.Role == "Admin") return true;
            var membership = TeamMembers.FirstOrDefault(tm => tm.UserId == user.Id && tm.TeamId == team.Id);
            return membership?.Role == "Founder" || membership?.Role == "Admin" || membership?.Role == "Coach";
        }
    }

    public class DataWrapper
    {
        public List<User> Users { get; set; }
        public List<Team> Teams { get; set; }
        public List<TeamMember> TeamMembers { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<Session> Sessions { get; set; }
        public List<Attendance> Attendances { get; set; }
        public List<FundTransaction> Transactions { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}

