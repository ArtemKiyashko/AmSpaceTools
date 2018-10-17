using AmSpaceClient;
using AmSpaceModels.Enums;
using AmSpaceModels.Organization;
using AmSpaceModels.Sap;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using AutoMapper;
using ExcelWorker;
using ExcelWorker.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class PeopleUploadViewModel : BaseViewModel
    {
        private readonly IAmSpaceClient _client;
        private readonly IMapper _mapper;
        private readonly IExcelWorker _excelWorker;
        private string _fileName;
        private DataTable _workSheet;
        private ObservableCollection<SapPersonExcelRow> _inputRows;
        private readonly SearchPeopleViewModel _searchVm;

        public ObservableCollection<SapPersonExcelRow> InputRows
        {
            get { return _inputRows; }
            set { _inputRows = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsUploadVisible)); }
        }
        public ICommand OpenFileCommand { get; set; }

        public ICommand UploadDataCommand { get; set; }

        public bool IsUploadVisible
        {
            get
            {
                if (!InputRows.Any()) return false;
                return true;
            }
        }

        public PeopleUploadViewModel(IAmSpaceClient client, IMapper mapper, IExcelWorker excelWorker, SearchPeopleViewModel searchVm)
        {
            _client = client;
            _mapper = mapper;
            _excelWorker = excelWorker;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
            _searchVm = searchVm;
            InputRows = new ObservableCollection<SapPersonExcelRow>();
            InputRows.CollectionChanged += InputRows_CollectionChanged;
        }

        private void InputRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsUploadVisible));
        }

        private async void UploadData(object obj)
        {
            var view = new SearchPeople()
            {
                DataContext = _searchVm
            };
            var result = (bool)await DialogHost.Show(view, "RootDialog");
            if (!result) return;
            var topLvlManager = _searchVm.SelectedUser;
            IsLoading = true;
            var domainTree = await _client.GetOrganizationStructureAsync();
            var flatDomains = domainTree.Descendants(_ => _.Children);
            var inputRowsGroupedByContracts = InputRows.GroupBy(_ => new { _.IdentityNumber, _.ManagerId }, v => v);
            if (inputRowsGroupedByContracts.Count(_ => string.IsNullOrEmpty(_.Key.ManagerId)) > 1)
                throw new ArgumentException($"More than one person has empty {nameof(SapPersonExcelRow.ManagerId)}", nameof(SapPersonExcelRow.ManagerId));
            var tree = inputRowsGroupedByContracts.GenerateTree(c => c.Key.IdentityNumber, c => c.Key.ManagerId ?? string.Empty, string.Empty);
            foreach (var account in tree.Descendants(_ => _.Children))
            {
                foreach(var contract in account.Item)
                {
                    var currentUserDomain = GetUserDomain(flatDomains, contract);
                    var externalAccount = FillAccount(topLvlManager, contract, currentUserDomain);
                    var accountResult = await UploadAccount(contract, externalAccount);
                }
            }
            IsLoading = false;
        }

        private AmspaceDomain GetUserDomain(IEnumerable<AmspaceDomain> flatDomains, SapPersonExcelRow contract)
        {
            var currentUserDomain = flatDomains.FirstOrDefault(_ => _.Mpk == contract.Mpk);
            if (currentUserDomain == null) throw new ArgumentNullException(nameof(SapPersonExcelRow.Mpk), $"Cannot find domain with MPK {contract.Mpk} for User {contract.Name} {contract.Surname}");
            return currentUserDomain;
        }

        private async Task<ExternalAccount> UploadAccount(SapPersonExcelRow contract, ExternalAccount externalAccount)
        {
            var accountResult = new ExternalAccount();
            var existingUser = await _client.FindUserByIdentityNumber(contract.IdentityNumber);
            if (existingUser == null)
                accountResult = await _client.CreateExternalAccount(externalAccount);
            else
            {
                var existingContract = existingUser.Contracts.Find(_ => _.ContractNumber == contract.ContractNumber);
                if (existingContract == null)
                    accountResult = await _client.CreateExternalAccount(externalAccount);
                else
                    accountResult = await _client.UpdateExternalAccount(existingContract.Id, externalAccount);
            }
            return accountResult;
        }

        private ExternalAccount FillAccount(SearchUserResultWithContractViewModel topLvlManager, SapPersonExcelRow contract, AmspaceDomain currentUserDomain)
        {
            var externalAccount = _mapper.Map<ExternalAccount>(contract);
            externalAccount.DomainId = currentUserDomain.Id;
            if (string.IsNullOrEmpty(externalAccount.ManagerLegalId)) externalAccount.ManagerLegalId = topLvlManager.User.PersonLegalId;
            return externalAccount;
        }

        private void OpenFile(object obj)
        {
            IsLoading = true;
            var dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                _fileName = dialog.FileName;
                _excelWorker.OpenFile(_fileName);
                _workSheet = _excelWorker.GetWorkSheet(1);
                _excelWorker.ExctractData<SapPersonExcelRow>(_workSheet.TableName).ForEach(_ => InputRows.Add(_));
            }
            IsLoading = false;
        }
    }
}
