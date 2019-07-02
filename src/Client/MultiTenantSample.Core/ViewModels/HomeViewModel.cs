using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MultiTenantSample.Core.Repositories;
using MultiTenantSample.Models;
using Robo.Mvvm.Input;
using Robo.Mvvm.Services;

namespace MultiTenantSample.Core.ViewModels
{
    public class HomeViewModel : BaseNavigationViewModel
    {
        string _tenantId;
        public string TenantId
        {
            get => _tenantId;
            set => SetPropertyChanged(ref _tenantId, value);
        }

        public string LoggedInAs => AppInstance.CurrentUser;

        // In case we want to add (observed) individual additions later
        ObservableCollection<Item> _items = new ObservableCollection<Item>();
        public ObservableCollection<Item> Items
        {
            get => _items;
            set => SetPropertyChanged(ref _items, value);
        }

        public ITestRepository TestRepository { get; set; }

        ICommand _addItemCommand;
        public ICommand AddItemCommand
        {
            get
            {
                if (_addItemCommand == null)
                {
                    _addItemCommand = new Command(AddItem);
                }

                return _addItemCommand;
            }
        }

        ICommand _deleteItemCommand;
        public ICommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new Command<Item>((item) => DeleteItem(item));
                }

                return _deleteItemCommand;
            }
        }

        ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new Command(Logout);
                }

                return _logoutCommand;
            }
        }

        public HomeViewModel(INavigationService navigationService, ITestRepository testRepository) : base(navigationService)
        {
            TestRepository = testRepository;
        }

        public override async Task InitAsync()
        {
            IsBusy = true;

            TenantId = AppInstance.CurrentSession.TenantId;

            await TestRepository.StartReplication().ConfigureAwait(false);

            var items = await Task.Run(() => TestRepository?.GetItems(UpdateItems));

            if (items?.Count > 0)
            {
                UpdateItems(items);
            }

            IsBusy = false;
        }

        void AddItem()
        {
            var item = new Item
            {
                Name = Guid.NewGuid().ToString(),
                CreatedBy = AppInstance.CurrentUser,
                CreatedDateTime = DateTime.Now,
                Channels = new string[] { AppInstance.CurrentSession.TenantId }
            };

            TestRepository.SaveItem(item);
        }

        void DeleteItem(Item item) => TestRepository.DeleteItem(item);

        void UpdateItems(List<Item> items)
        {
            items = items.OrderByDescending(x => x.CreatedDateTime).ToList();
            Items = new ObservableCollection<Item>(items);
        }

        void Logout()
        {
            AppInstance.Reset();
            TestRepository.Dispose();
            Navigation.SetRoot<LoginViewModel>(false);
        }
    }
}
