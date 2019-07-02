using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MultiTenantSample.Core.Services;
using MultiTenantSample.Models;
using Robo.Mvvm.Input;
using Robo.Mvvm.Services;

namespace MultiTenantSample.Core.ViewModels
{
    public class LoginViewModel : BaseNavigationViewModel
    {
        string _username;
        public string Username
        {
            get => _username;
            set => SetPropertyChanged(ref _username, value);
        }

        string _password;
        public string Password
        {
            get => _password;
            set => SetPropertyChanged(ref _password, value);
        }

        ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new Command(async () => await LoginAsync());
                }

                return _loginCommand;
            }
        }

        IAuthenticationService AuthenticationService { get; set; }

        public LoginViewModel(INavigationService navigationService, IAuthenticationService authenticationService) : base(navigationService)
        {
            AuthenticationService = authenticationService;
        }

        async Task LoginAsync()
        {
            // Feigning a login with our AuthApi
            var credentials = new Credentials
            {
                Username = Username,
                Password = Password
            };

            var session = await AuthenticationService.Authenticate(credentials);

            if (!string.IsNullOrEmpty(session?.session_id) &&
                !string.IsNullOrEmpty(session?.TenantId))
            {
                AppInstance.CurrentUser = Username;
                AppInstance.CurrentSession = session; 
                Navigation.SetRoot<HomeViewModel>(true);
            }
        }
    }
}
