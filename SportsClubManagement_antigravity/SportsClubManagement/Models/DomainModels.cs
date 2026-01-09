using System;
using System.Collections.Generic;

namespace SportsClubManagement.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; }
        public string Password { get; set; } // For demo only - should be hashed in production
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "User"; // "Admin", "User"
        public string AvatarPath { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class TeamMember
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string TeamId { get; set; }
        public string Role { get; set; } = "Member"; // "Founder", "Admin", "Coach", "Member"
        public DateTime JoinDate { get; set; } = DateTime.Now;
    }

    public class Subject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeamId { get; set; }
        public string UserId { get; set; } // Nullable, if set, it's a personal subject
        public string Name { get; set; }
        public string Description { get; set; }
        public int ParticipantCount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class Session
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeamId { get; set; }
        public string UserId { get; set; } // Nullable, if set, it's a personal session
        public string SubjectId { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Note { get; set; }
        public bool IsAttended { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class Attendance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public bool IsPresent { get; set; }
        public string Note { get; set; }
        public DateTime RecordedDate { get; set; } = DateTime.Now;
    }

    public class FundTransaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeamId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; } // Positive = Deposit, Negative = Withdraw
        public string Description { get; set; }
        public string ByUserId { get; set; }
        public string Type { get; set; } = "Deposit"; // "Deposit", "Withdraw"
    }

    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeamId { get; set; }
        public string ByUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsSystemNotification { get; set; } = false;
        public string Title { get; set; }
    }
}
