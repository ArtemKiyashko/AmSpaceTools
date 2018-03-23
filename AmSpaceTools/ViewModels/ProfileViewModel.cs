using AmSpaceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _jobTitle;
        private string _profileImageLink;

        public ProfileViewModel(IAmSpaceClient amSpaceClient)
        {
            var model = amSpaceClient.ProfileRequestAsync().Result;
            Name = $"{model.FirstName} {model.LastName}";
            JobTitle = model.ContractData.FirstOrDefault().Position.Name;
            ProfileImageLink = model.Avatar;
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
    }
}