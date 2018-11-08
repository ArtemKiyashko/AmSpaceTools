using AmSpaceTools.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private BaseViewModel _selectedViewModel;
        private ProfileViewModel _profileViewModel;
        private MenuItem _selectedMenuItem;
        private bool _isLoading;
        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();


        protected BaseViewModel()
        {
            ValidateModel();
        }

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

        public bool DefaultButtonIsEnabled { get => !HasErrors; }
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
                OnPropertyChanged(nameof(UIEnabled));
            }
        }

        public bool UIEnabled { get { return !IsLoading; } }

        public MainWindowViewModel MainViewModel
        {
            get
            {
                return Services.Container.GetInstance<MainWindowViewModel>();
            }
        }

        public bool HasErrors => _validationErrors.Any();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void OnErrorsChanged([CallerMemberName] string propName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propName));
            OnPropertyChanged(nameof(DefaultButtonIsEnabled));
        }

        protected void ValidateModelProperty([CallerMemberName] string propName = null)
        {
            if (_validationErrors.ContainsKey(propName))
                _validationErrors.Remove(propName);
            var value = GetType().GetProperty(propName).GetValue(this);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(this, null, null) { MemberName = propName };
            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                _validationErrors.Add(propName, new List<string>());
                foreach (ValidationResult validationResult in validationResults)
                {
                    _validationErrors[propName].Add(validationResult.ErrorMessage);
                }
            }
            OnErrorsChanged(propName);
        }

        protected void ValidateModel()
        {
            _validationErrors.Clear();
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(this, null, null);
            if (!Validator.TryValidateObject(this, validationContext, validationResults, true))
            {
                foreach (ValidationResult validationResult in validationResults)
                {
                    string property = validationResult.MemberNames.ElementAt(0);
                    if (_validationErrors.ContainsKey(property))
                    {
                        _validationErrors[property].Add(validationResult.ErrorMessage);
                    }
                    else
                    {
                        _validationErrors.Add(property, new List<string> { validationResult.ErrorMessage });
                    }
                }
            }
            _validationErrors.ForEach(_ => OnErrorsChanged(_.Key));
        }

        /// <summary>
        /// Updates app's views after Login according to specified startup View and profile model
        /// </summary>
        /// <param name="nextView"></param>
        protected void ShowMenu(BaseViewModel startupViewModel)
        {
            MainViewModel.SelectedViewModel = startupViewModel;
            MainViewModel.MenuItems.Clear();
            MainViewModel.MenuItems.Add(new MenuItem("IDP Translation", startupViewModel));
            MainViewModel.MenuItems.Add(new MenuItem("Org. Structure", Services.Container.GetInstance<OrgStructureViewModel>()));
            MainViewModel.MenuItems.Add(new MenuItem("People Batch Upload", Services.Container.GetInstance<PeopleUploadViewModel>()));
            MainViewModel.SelectedMenuItem = MainViewModel.MenuItems.FirstOrDefault(item => item.Content == startupViewModel);
        }

        protected void HideMenu()
        {
            var loginVm = Services.Container.GetInstance<LoginViewModel>();
            MainViewModel.SelectedViewModel = loginVm;
            MainViewModel.MenuItems.Clear();
            MainViewModel.MenuItems.Add(new MenuItem("Login", loginVm));
            MainViewModel.SelectedMenuItem = MainViewModel.MenuItems.FirstOrDefault(item => item.Content == loginVm);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
            || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }
    }
}
