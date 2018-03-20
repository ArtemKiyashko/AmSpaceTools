using AmSpaceClient;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private ICommand _loginCommand;
        private ICommand _changeViewCommand;
        private SecureString _password;
        private string _name;
        private IAmSpaceClient _client;

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

        public ICommand ChangeViewCommand
        {
            get
            {
                return _changeViewCommand;
            }
            set
            {
                _changeViewCommand = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public SecureString Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public LoginViewModel(IAmSpaceClient client)
        {
            _client = client;
            LoginCommand = new RelayCommand(Login);
            _changeViewCommand = new RelayCommand(ChangeView);
            IsLoading = false;
        }

        private void Login(object obj)
        {
            var passwordContainer = obj as IHavePassword;
            if (passwordContainer != null)
            {
                Password = passwordContainer.Password;
            }
            LoginRequest();
        }

        private void ChangeView(object obj)
        {
            var viewModelContainer = obj as BaseViewModel;
            if (viewModelContainer != null)
            {
                MainViewModel.SelectedViewModel = viewModelContainer;
            }
        }

        private async void LoginRequest()
        {
            IsLoading = true;
            var result = await _client.LoginRequestAsync(Name, Password);
            if (!result) throw new Exception();
            var profileModel = await _client.ProfileRequestAsync();
            MainViewModel.SelectedViewModel = Services.Container.GetInstance<IdpTranslationsPreviewViewModel>();
            MainViewModel.MenuItems.Remove(MainViewModel.MenuItems.FirstOrDefault());
            MainViewModel.MenuItems.Add(new MenuItem("IDP Translation", Services.Container.GetInstance<IdpTranslationsPreviewViewModel>()));
            IsLoading = false;
        }
    }
}
