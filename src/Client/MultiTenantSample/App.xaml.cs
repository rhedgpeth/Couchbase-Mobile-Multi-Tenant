using MultiTenantSample.Core.Repositories;
using MultiTenantSample.Core.Services;
using MultiTenantSample.Core.ViewModels;
using MultiTenantSample.Repositories;
using MultiTenantSample.Services;
using Robo.Mvvm;
using Xamarin.Forms;

namespace MultiTenantSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            ServiceContainer.Register<ITestRepository>(new TestRepository());
            ServiceContainer.Register<IAuthenticationService>(new AuthenticationService());

            Robo.Mvvm.Forms.App.Init<LoginViewModel>(GetType().Assembly);
        }
    }
}
