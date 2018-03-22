using AmSpaceTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private BaseViewModel _selectedViewModel;
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
                
                OnPropertyChanged(nameof(SelectedViewModel));
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
                OnPropertyChanged(nameof(IsLoading));
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

        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
