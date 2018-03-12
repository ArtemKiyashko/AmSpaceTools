﻿using AmSpaceClient;
using AmSpaceTools.Infrastructure;
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
        private SecureString _password;
        private string _name;
        public MainWindowViewModel MainViewModel
        {
            get
            {
                return Services.Container.GetInstance<MainWindowViewModel>();
            }
        }

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
            LoginRequest();
        }

        private async void LoginRequest()
        {
            IsLoading = true;
            var client = Services.Container.GetInstance<IAmSpaceClient>();
            var result = await client.LoginRequestAsync(Name, Password);
            if (!result) throw new Exception();
            var profileModel = await client.ProfileRequestAsync();
            MainViewModel.SelectedViewModel = new ProfileViewModel();
            IsLoading = false;
        }
    }
}
