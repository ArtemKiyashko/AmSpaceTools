using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private List<MenuItem> _menuItems;
        public List<MenuItem> MenuItems
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

            MenuItems = new List<MenuItem>
            {
                new MenuItem("Login", Services.Container.GetInstance<LoginViewModel>())
            };
        }
    }
}
