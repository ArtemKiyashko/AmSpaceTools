using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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
            Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            SelectedViewModel = Services.Container.GetInstance<LoginViewModel>();
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem("Login", SelectedViewModel)
            };
            SelectedMenuItem = MenuItems[0];
        }

        private void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var view = new Error
            {
                DataContext = Services.Container.With<string>(e.Exception.Message).GetInstance<ErrorViewModel>()
            };
            DialogHost.Show(view, "RootDialog", delegate(object s, DialogOpenedEventArgs args)
            {
                SelectedViewModel.IsLoading = false;
                e.Handled = true;
            });
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
