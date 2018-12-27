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
        private bool _isLoading;
        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();

        protected BaseViewModel()
        {
            ValidateModel();
        }

        public bool DefaultButtonIsEnabled { get => !HasErrors; }
        
        public bool IsLoading
        {
            get => _isLoading;
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
            get => Services.Container.GetInstance<MainWindowViewModel>();
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

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
            || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }
    }
}
