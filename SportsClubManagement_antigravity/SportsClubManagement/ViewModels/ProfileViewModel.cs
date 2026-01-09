using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;
using System.IO;
using Microsoft.Win32;

namespace SportsClubManagement.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private User _currentUser;
        private string _username;
        private string _fullName;
        private string _email;
        private string _avatarPath;
        private DateTime _birthDate;
        private string _message;
        private string _currentPassword;
        private string _newPassword;
        private string _confirmPassword;
        private bool _isChangingPassword;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string AvatarPath
        {
            get => _avatarPath;
            set => SetProperty(ref _avatarPath, value);
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsChangingPassword
        {
            get => _isChangingPassword;
            set => SetProperty(ref _isChangingPassword, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand SelectAvatarCommand { get; }

        public ProfileViewModel()
        {
            _currentUser = DataService.Instance.CurrentUser;
            if (_currentUser != null)
            {
                LoadUserData();
            }
            SaveCommand = new RelayCommand(SaveProfile);
            CancelCommand = new RelayCommand(o => LoadUserData());
            ChangePasswordCommand = new RelayCommand(ChangePassword, CanChangePassword);
            SelectAvatarCommand = new RelayCommand(SelectAvatar);
        }

        private void LoadUserData()
        {
            if (_currentUser != null)
            {
                Username = _currentUser.Username;
                FullName = _currentUser.FullName;
                Email = _currentUser.Email;
                AvatarPath = _currentUser.AvatarPath ?? "https://via.placeholder.com/150";
                BirthDate = _currentUser.BirthDate;
                Message = "";
            }
        }

        private void SaveProfile(object obj)
        {
            if (!ValidateProfile())
                return;

            if (_currentUser != null)
            {
                _currentUser.FullName = FullName;
                _currentUser.Email = Email;
                _currentUser.AvatarPath = AvatarPath;
                _currentUser.BirthDate = BirthDate;
                DataService.Instance.Save();
                Message = "Cập nhật thông tin thành công!";
            }
        }

        private bool ValidateProfile()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                Message = "Vui lòng nhập họ tên";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            {
                Message = "Email không hợp lệ";
                return false;
            }

            if (BirthDate > DateTime.Now)
            {
                Message = "Ngày sinh không hợp lệ";
                return false;
            }

            return true;
        }

        private bool CanChangePassword(object obj)
        {
            return !string.IsNullOrWhiteSpace(CurrentPassword) && 
                   !string.IsNullOrWhiteSpace(NewPassword) && 
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private void SelectAvatar(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourcePath = openFileDialog.FileName;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
                    
                    // Determine project directory
                    string appDir = AppDomain.CurrentDomain.BaseDirectory;
                    // Usually: bin\Debug\netX.X-windows\
                    // We want: SportsClubManagement\Data\Avatar
                    
                    DirectoryInfo dirInfo = new DirectoryInfo(appDir);
                    // Navigate up to SportsClubManagement folder
                    string projectDir = dirInfo.Parent.Parent.Parent.FullName;
                    string avatarDir = Path.Combine(projectDir, "Data", "Avatar");
                    
                    if (!Directory.Exists(avatarDir))
                    {
                        Directory.CreateDirectory(avatarDir);
                    }
                    
                    string destPath = Path.Combine(avatarDir, fileName);
                    File.Copy(sourcePath, destPath, true);
                    
                    // Update current view model path
                    // Use relative path or absolute path? 
                    // To be safe for display, use absolute path.
                    AvatarPath = destPath;
                    
                    Message = "Avatar đã được chọn. Hãy bấm 'Lưu thay đổi' để hoàn tất.";
                }
                catch (Exception ex)
                {
                    Message = "Lỗi khi thay đổi avatar: " + ex.Message;
                }
            }
        }

        private void ChangePassword(object obj)
        {
            if (_currentUser == null) return;

            // Verify current password
            if (_currentUser.Password != CurrentPassword)
            {
                Message = "Mật khẩu hiện tại không chính xác!";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                Message = "Mật khẩu xác nhận không khớp!";
                return;
            }

            if (NewPassword.Length < 6)
            {
                Message = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return;
            }

            // Update password
            _currentUser.Password = NewPassword;
            DataService.Instance.Save();
            
            CurrentPassword = "";
            NewPassword = "";
            ConfirmPassword = "";
            IsChangingPassword = false;
            Message = "Đổi mật khẩu thành công!";
        }
    }
}
