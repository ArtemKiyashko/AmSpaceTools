using AmSpaceClient;
using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AmSpaceTools.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _jobTitle;
        private string _profileImageLink;
        private ICommand _logoutCommand;
        private IAmSpaceClient _client;
        private BitmapSource _avatar;
        private Profile _profile;

        public ProfileViewModel(IAmSpaceClient client)
        {
            _client = client;
            _profile = _client.ProfileRequestAsync().Result;
            _profileImageLink = _profile.Avatar;
            Name = $"{_profile.FirstName} {_profile.LastName}";
            JobTitle = _profile.ContractData.FirstOrDefault().Position.Name;
            SetUpAvatar();
            LogoutCommand = new RelayCommand(LogOut);
        }

        private async void LogOut(object obj)
        {
            await _client.LogoutRequestAsync();
            MainViewModel.IsLoggedIn = false;
            HideMenu();
        }

        protected async void SetUpAvatar()
        {
            Avatar = await _client.GetAvatarAsync(_profileImageLink);
        }
        public BitmapSource Avatar
        {
            get
            {
                return _avatar;
            }
            set
            {
                _avatar = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string JobTitle
        {
            get
            {
                return _jobTitle;
            }
            set
            {
                _jobTitle = value;
                OnPropertyChanged(nameof(JobTitle));
            }
        }

        public ICommand LogoutCommand{ get => _logoutCommand; set => _logoutCommand = value; }
    }
}