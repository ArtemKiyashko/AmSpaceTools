using AmSpaceClient;
using AmSpaceModels.Auth;
using AmSpaceTools.Infrastructure;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private IAmSpaceClient _client;
        private string _newPassword;
        private string _rePassword;

        [Required]
        public string NewPassword { get => _newPassword;
            set
            {
                _newPassword = value;
                ValidateModelProperty();
            }
        }

        [Required]
        public string RePassword { get => _rePassword;
            set
            {
                _rePassword = value;
                ValidateModelProperty();
            }
        }

        public NewPassword Password { get; set; }

        public RelayCommand ApplyCommand { get; }

        public string DescriptionText { get; set; }

        public bool DescriptionVisible => !string.IsNullOrEmpty(DescriptionText);

        public ChangePasswordViewModel(IAmSpaceClient client)
        {
            _client = client;
            ApplyCommand = new RelayCommand(Apply);
        }

        private void Apply(object obj)
        {
            Password = new NewPassword
            {
                Password = NewPassword,
                RePassword = RePassword
            };
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
