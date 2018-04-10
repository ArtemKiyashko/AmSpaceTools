using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private ObservableCollection<MenuItem> _menuItems;
        public ObservableCollection<MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                _menuItems = value;
                OnPropertyChanged(nameof(MenuItems));
            }
        }
        public MainWindowViewModel()
        {
            SelectedViewModel = Services.Container.GetInstance<LoginViewModel>();
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem("Login", SelectedViewModel)
            };
            SelectedMenuItem = MenuItems[0];
        }

        private bool _isLoggedIn;

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }
    }
}
