using AmSpaceClient;
using AmSpaceClient.Extensions;
using AmSpaceModels;
using AmSpaceModels.Auth;
using AmSpaceTools.Infrastructure;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AmSpaceTools.ViewModels
{
    public class MsWebLoginViewModel : BaseViewModel
    {
        private readonly IAmSpaceClient _client;
        private IAsyncCommand<Uri> _navigationCommand;
        private IAmSpaceEnvironment _currentEnvironment;

        public IAsyncCommand<Uri> NavigationCommand
        {
            get { return _navigationCommand; }
            set
            {
                if (_navigationCommand != value)
                {
                    _navigationCommand = value;
                    OnPropertyChanged(nameof(NavigationCommand));
                }
            }
        }
        public string SourceUrl { get; set; }

        public MsWebLoginViewModel(IAmSpaceClient client)
        {
            _client = client;
            NavigationCommand = new RelayCommandAsync<Uri>(Navigation);
        }

        private async Task Navigation(Uri arg)
        {
            if (_currentEnvironment.SsoOptions.RedirectUrl.Contains(arg.Host))
            {
                var uriBuilder = new UriBuilder(arg);
                var result = uriBuilder.GetObject<LoginCodeFlowAuthResult>();
                try
                {
                    MainViewModel.IsLoggedIn = await _client.LoginWithCodeFlow(result, _currentEnvironment);
                }
                catch
                {
                    MainViewModel.HideMenu();
                    throw;
                }
            }
        }

        public void InitiateLogin(IAmSpaceEnvironment environment)
        {
            _currentEnvironment = environment;
            var uriBuilder = new UriBuilder($"https://login.microsoftonline.com/{_currentEnvironment.SsoOptions.TenantId}/oauth2/v2.0/authorize");
            uriBuilder.AddQueryNotNull("response_type", "code");
            uriBuilder.AddQueryNotNull("client_id", _currentEnvironment.SsoOptions.ClientId);
            uriBuilder.AddQueryNotNull("redirect_uri", _currentEnvironment.SsoOptions.RedirectUrl);
            uriBuilder.AddQueryNotNull("scope", _currentEnvironment.SsoOptions.Scopes.Aggregate((current, next) => $"{current}+{next}"));
            //TODO: determine how to generate state property
            uriBuilder.AddQueryNotNull("state", "7orcc5cwAR6gyd4FkBxefI3SzJPNSHdumd1cfFKnij8r11n8R7aGMuOHb4AnumsN:1in0P8:pDV4NxHNvESfKOZEREuQuRNNKMw");
            uriBuilder.AddQueryNotNull("response_mode", "query");
            uriBuilder.AddQueryNotNull("prompt", "login");
            SourceUrl = uriBuilder.ToString();
            MainViewModel.SelectedViewModel = this;
        }
    }
}
