using AmSpaceTools.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmSpaceTools.Infrastructure
{

    /// <summary>
    /// Item of left-side menu
    /// </summary>
    public class MenuItem : BaseViewModel
    {
        private string _name;
        private BaseViewModel _content;
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        private Thickness _marginRequirement = new Thickness(16);
        private ICommand _changeViewCommand;
        
        public MenuItem(string name, BaseViewModel content)
        {
            _name = name;
            Content = content;
            _changeViewCommand = new RelayCommand(ChangeView);
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public BaseViewModel Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get
            {
                return _horizontalScrollBarVisibilityRequirement;
            }
            set
            {
                _horizontalScrollBarVisibilityRequirement = value;
                OnPropertyChanged(nameof(HorizontalScrollBarVisibilityRequirement));
            }
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get
            {
                return _verticalScrollBarVisibilityRequirement;
            }
            set
            {
                _verticalScrollBarVisibilityRequirement = value;
                OnPropertyChanged(nameof(VerticalScrollBarVisibilityRequirement));
            }
        }

        public Thickness MarginRequirement
        {
            get
            {
                return _marginRequirement;
            }
            set
            {
                _marginRequirement = value;
                OnPropertyChanged(nameof(MarginRequirement));
            }
        }

        public ICommand ChangeViewCommand
        {
            get
            {
                return _changeViewCommand;
            }
            set
            {
                _changeViewCommand = value;
            }
        }

        private void ChangeView(object obj)
        {
            MainViewModel.SelectedViewModel = Content;
            MainViewModel.SelectedMenuItem = this;
        }
    }
}
