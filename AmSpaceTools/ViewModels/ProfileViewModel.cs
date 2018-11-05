using AmSpaceClient;
using AmSpaceModels;
using AmSpaceModels.Performance;
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
            SetUpProfile();
            LogoutCommand = new RelayCommand(LogOut);
        }

        private async void LogOut(object obj)
        {
            IsLoading = true;
            var result = await _client.LogoutRequestAsync();
            if (!result) throw new Exception();
            MainViewModel.IsLoggedIn = false;
            HideMenu();
            IsLoading = false;
        }

        protected async void SetUpProfile()
        {
            _profile = await _client.GetProfileAsync();
            _profileImageLink = _profile.Avatar;
            Name = $"{_profile.FirstName} {_profile.LastName}";
            JobTitle = _profile.ContractData.FirstOrDefault().Position.Name;
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