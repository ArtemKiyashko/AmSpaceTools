using AmSpaceClient;
using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private ICommand _loginCommand;
        private SecureString _password;
        private string _name;
        private IAmSpaceClient _client;
        private IEnumerable<AmSpaceEnvironment> _environments;
        private AmSpaceEnvironment _selectedEnvironment;

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand;
            }
            set
            {
                _loginCommand = value;
            }
        }

        public AmSpaceEnvironment SelectedEnvironment
        {
            get
            {
                if (_selectedEnvironment == null)
                    _selectedEnvironment = Environments.FirstOrDefault();
                return _selectedEnvironment;
            }
            set
            {
                _selectedEnvironment = value;
            }
        }

        [Required]
        public string Name
        {
            get { return _name; }
            set { _name = value; ValidateModelProperty(); }
        }

        public SecureString Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public IEnumerable<AmSpaceEnvironment> Environments { get => _environments; set => _environments = value; }

        public LoginViewModel(IAmSpaceClient client, IAmSpaceEnvironmentsProvider environmenProvider)
        {
            _client = client;
            _environments = environmenProvider.Environments;
            LoginCommand = new RelayCommand(Login);
            IsLoading = false;
        }

        private void Login(object obj)
        {
            var passwordContainer = obj as IHavePassword;
            if (passwordContainer != null)
            {
                Password = passwordContainer.Password;
            }
            if (string.IsNullOrEmpty(Name) || SelectedEnvironment == null) return;
            LoginRequest();
        }

        private async void LoginRequest()
        {
            IsLoading = true;
            MainViewModel.IsLoggedIn = await _client.LoginRequestAsync(Name, Password, SelectedEnvironment);
            MainViewModel.ProfileViewModel = Services.Container.GetInstance<ProfileViewModel>();
            var ipdTranslationViewModel = Services.Container.GetInstance<IdpTranslationsPreviewViewModel>();
            MainViewModel.ShowMenu(ipdTranslationViewModel);
            IsLoading = false;
        }
    }
}
