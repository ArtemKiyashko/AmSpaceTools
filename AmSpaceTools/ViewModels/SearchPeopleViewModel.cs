using AmSpaceClient;
using AmSpaceModels.Enums;
using AmSpaceModels.Organization;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Infrastructure.Extensions;
using ExcelWorker.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class SearchPeopleViewModel : BaseViewModel
    {
        private IAmSpaceClient _client;
        private IEnumerable<SearchUserResult> _searchResult;
        private SearchUserResultWithContractViewModel _selectedUser;
        private string _managerName;
        private string _managerDomain;
        private SapPersonExcelRow _subordinate;

        public ObservableCollection<SearchUserResultWithContractViewModel> SearchResultWithContract { get; set; }

        public RelayCommand SearchCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand ApplyCommand { get; }

        public SapPersonExcelRow Subordinate
        {
            get { return _subordinate; }
            set { _subordinate = value; }
        }

        public string SubordinateText => $"For {Subordinate?.Name} {Subordinate?.Surname} [{Subordinate?.IdentityNumber}]";

        public bool SubordinateVisible => Subordinate != null;

        [Required(ErrorMessage = "Please specify manager name")]
        public string ManagerName
        {
            get { return _managerName; }
            set
            {
                _managerName = value;
                SelectedUser = null;
                OnPropertyChanged();
                ValidateModelProperty();
                OnPropertyChanged(nameof(SearchButtonVisible));
                OnPropertyChanged(nameof(ApplyButtonVisible));
            }
        }
        public string ManagerDomain
        {
            get { return _managerDomain; }
            set { _managerDomain = value; OnPropertyChanged(); }
        }
        public SearchUserResultWithContractViewModel SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; OnPropertyChanged(nameof(SearchButtonVisible)); OnPropertyChanged(nameof(ApplyButtonVisible)); }
        }
        public bool SearchButtonVisible
        {
            get
            {
                if (SelectedUser == null)
                    return true;
                return false;
            }
        }
        public bool ApplyButtonVisible
        {
            get
            {
                return !SearchButtonVisible;
            }
        }

        public IEnumerable<SearchUserResult> SearchResult
        {
            get { return _searchResult; }
            set { _searchResult = value; OnPropertyChanged(); }
        }

        public SearchPeopleViewModel(IAmSpaceClient client)
        {
            _client = client;
            SearchCommand = new RelayCommand(Search);
            ClearCommand = new RelayCommand(Clear);
            ApplyCommand = new RelayCommand(Apply);
            SearchResultWithContract = new ObservableCollection<SearchUserResultWithContractViewModel>();
        }

        private void Apply(object obj)
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
        }

        private void Clear(object obj)
        {
            ManagerName = string.Empty;
            ManagerDomain = string.Empty;
            SearchResult = null;
            SelectedUser = null;
            SearchResultWithContract.Clear();
        }

        private async void Search(object obj)
        {
            IsLoading = true;
            SearchResultWithContract.Clear();
            SearchResult = await _client.FindUsers(ManagerName, null, null, AmSpaceUserStatus.ACTIVE, ManagerDomain, null);
            SearchResult.ForEach(_ => SearchResultWithContract.Add(new SearchUserResultWithContractViewModel {
                User = _,
                MainContract = _.Contracts.First()
            }));
            IsLoading = false;
        }
    }
}
