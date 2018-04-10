using AmSpaceClient;
using AmSpaceTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _jobTitle;
        private string _profileImageLink;
        private ICommand _logoutCommand;
        private IAmSpaceClient _client;

        public ProfileViewModel(IAmSpaceClient client)
        {
            _client = client;
            var model = _client.ProfileRequestAsync().Result;
            Name = $"{model.FirstName} {model.LastName}";
            JobTitle = model.ContractData.FirstOrDefault().Position.Name;
            ProfileImageLink = model.Avatar;
            LogoutCommand = new RelayCommand(LogOut);
        }

        private async void LogOut(object obj)
        {
            await _client.LogoutRequestAsync();
            MainViewModel.IsLoggedIn = false;
            HideMenu();
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

        public string ProfileImageLink
        {
            get
            {
                return _profileImageLink;
            }
            set
            {
                _profileImageLink = value;
                OnPropertyChanged(nameof(ProfileImageLink));
            }
        }

        public ICommand LogoutCommand{ get => _logoutCommand; set => _logoutCommand = value; }
    }
}