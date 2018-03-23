using AmSpaceTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private BaseViewModel _selectedViewModel;
        private ProfileViewModel _profileViewModel;
        private MenuItem _selectedMenuItem;
        private bool _isLoading;

        public BaseViewModel SelectedViewModel
        {
            get
            {
                return _selectedViewModel;
            }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged();
            }
        }

        public ProfileViewModel ProfileViewModel
        {
            get
            {
                return _profileViewModel;
            }
            set
            {
                _profileViewModel = value;
                OnPropertyChanged(nameof(ProfileViewModel));
            }
        }

        public MenuItem SelectedMenuItem
        {
            get
            {
                return _selectedMenuItem;
            }
            set
            {
                _selectedMenuItem = value;
                OnPropertyChanged(nameof(SelectedMenuItem));
            }
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel MainViewModel
        {
            get
            {
                return Services.Container.GetInstance<MainWindowViewModel>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
