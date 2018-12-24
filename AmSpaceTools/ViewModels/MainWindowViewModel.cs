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
        private BaseViewModel _selectedViewModel;
        private MenuItem _selectedMenuItem;
        private ProfileViewModel _profileViewModel;
        private bool _isLoggedIn;

        public ObservableCollection<MenuItem> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                OnPropertyChanged(nameof(MenuItems));
            }
        }

        public BaseViewModel SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged();
            }
        }

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem = value;
                OnPropertyChanged(nameof(SelectedMenuItem));
            }
        }

        public ProfileViewModel ProfileViewModel
        {
            get => _profileViewModel;
            set
            {
                _profileViewModel = value;
                OnPropertyChanged(nameof(ProfileViewModel));
            }
        }
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
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
            (SelectedViewModel as IProgressReporter)?.ProgressVM.CloseLoading();

            DialogHost.Show(view, "RootDialog", (object s, DialogOpenedEventArgs args) =>
            {
                SelectedViewModel.IsLoading = false;
            });
            e.Handled = true;
        }

        /// <summary>
        /// Updates app's views after Login according to specified startup View and profile model
        /// </summary>
        /// <param name="nextView"></param>
        internal void ShowMenu(BaseViewModel startupViewModel)
        {
            MainViewModel.SelectedViewModel = startupViewModel;
            MainViewModel.MenuItems.Clear();
            MainViewModel.MenuItems.Add(new MenuItem("IDP Translation", startupViewModel));
            MainViewModel.MenuItems.Add(new MenuItem("Org. Structure", Services.Container.GetInstance<OrgStructureViewModel>()));
            MainViewModel.MenuItems.Add(new MenuItem("People Batch Upload", Services.Container.GetInstance<PeopleUploadViewModel>()));
            MainViewModel.MenuItems.Add(new MenuItem("JobMap Batch Upload", Services.Container.GetInstance<JobMapUploadViewModel>()));
            MainViewModel.SelectedMenuItem = MainViewModel.MenuItems.FirstOrDefault(item => item.Content == startupViewModel);
        }

        internal void HideMenu()
        {
            var loginVm = Services.Container.GetInstance<LoginViewModel>();
            MainViewModel.SelectedViewModel = loginVm;
            MainViewModel.MenuItems.Clear();
            MainViewModel.MenuItems.Add(new MenuItem("Login", loginVm));
            MainViewModel.SelectedMenuItem = MainViewModel.MenuItems.FirstOrDefault(item => item.Content == loginVm);
        }
    }
}
