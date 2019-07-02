using MultiTenantSample.Core.ViewModels;
using MultiTenantSample.Models;
using Robo.Mvvm.Forms.Pages;
using Xamarin.Forms;

namespace MultiTenantSample.Pages
{
    public partial class HomePage : BaseContentPage<HomeViewModel>
    {
        public HomePage()
        {
            InitializeComponent();
        }

        void Handle_Context_Clicked(object sender, System.EventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var item = menuItem.CommandParameter as Item;

                if (item != null)
                {
                    ViewModel.DeleteItemCommand.Execute(item);
                }
            }
        }
    }
}
