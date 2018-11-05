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

        private async Task ValidateDomainsAsync(IEnumerable<SapPersonExcelRow> inputRows)
        {
            var domains = await _client.GetOrganizationStructureAsync();
            var flatDomains = domains.Descendants(_ => _.Children);
            var inputMpks = inputRows.Select(_ => _.Mpk).Distinct();
            var existingMpks = flatDomains.Select(_ => _.Mpk).Distinct();
            var missingMpks = inputMpks.Except(existingMpks);
            if (missingMpks.Any())
                throw new ArgumentException($"Those MPKs are missing: {missingMpks.Select(_ => _.ToString()).Aggregate((c, n) => c + ", " + n)}", nameof(SapPersonExcelRow.Mpk));

        }
        private async void UploadData(object obj)
        {
            IsLoading = true;
            await ValidateDomainsAsync(InputRows);
            var inputRowsGroupedByContracts = InputRows.GroupBy(_ => new { _.IdentityNumber, _.ManagerId }, v => v);
            var tree = inputRowsGroupedByContracts.GenerateTree(c => c.Key.IdentityNumber, c => c.Key.ManagerId ?? string.Empty, string.Empty);
            foreach (var account in tree.Descendants(_ => _.Children))
            {
                foreach(var contract in account.Item)
                {
                    var externalAccount = await FillAccount(contract);
                    var accountResult = await UploadAccount(externalAccount);
                }
            }
            IsLoading = false;
        }

        protected async Task<ExternalAccount> FillAccount(SapPersonExcelRow contract)
        {
            var externalAccount = _mapper.Map<ExternalAccount>(contract);
            await FillPosition(contract, externalAccount);
            await FillManager(contract, externalAccount);
            return externalAccount;
        }

        protected async Task FillPosition(SapPersonExcelRow contract, ExternalAccount externalAccount)
        {
            var positions = await _client.GetPositionsAsync();
            var existingPosition = positions
                .FirstOrDefault(_ => string.Equals(_.Name, contract.Position, StringComparison.OrdinalIgnoreCase));
            if (existingPosition == null)
                externalAccount.PositionName = contract.Position;
            else
            {
                externalAccount.PositionName = existingPosition.Name;
                externalAccount.PositionId = existingPosition.Id;
            }
        }

        protected async Task FillManager(SapPersonExcelRow contract, ExternalAccount externalAccount)
        {
            if (string.IsNullOrEmpty(contract.ManagerId))
            {
                _searchVm.Subordinate = contract;
                if (!string.IsNullOrEmpty(contract.AmRestManagerName))
                    _searchVm.ManagerName = contract.AmRestManagerName;

                var view = new SearchPeople()
                {
                    DataContext = _searchVm
                };
                var result = (bool)await DialogHost.Show(view, "RootDialog");
                if (!result) throw new ArgumentNullException(nameof(contract.ManagerId), $"Manager for {contract.Name} {contract.Surname} not set");
                externalAccount.ManagerId = _searchVm.SelectedUser.User.Id;
            }
            else
            {
                var manager = await _client.FindUserByIdentityNumber(contract.ManagerId);
                externalAccount.ManagerId = manager == null ? throw new ArgumentException($"Manager for {contract.Name} {contract.Surname} not found!") : manager.Id;
            }
        }

        protected async Task<ExternalAccountResponse> UploadAccount(ExternalAccount externalAccount)
        {
            var accountResult = new ExternalAccountResponse();
            var existingUser = await _client.FindUserByIdentityNumber(externalAccount.PersonLegalId);
            if (existingUser == null)
                accountResult = await _client.CreateExternalAccount(externalAccount);
            else
            {
                var existingContract = existingUser.Contracts.Find(_ => _.ContractNumber == externalAccount.ContractNumber);
                if (existingContract == null)
                    accountResult = await _client.CreateExternalAccount(externalAccount);
                else
                    accountResult = await _client.UpdateExternalAccount(existingContract.Id, externalAccount);
            }
            return accountResult;
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
                using (_excelWorker)
                {
                    _fileName = dialog.FileName;
                    _excelWorker.OpenFile(_fileName);
                    _workSheet = _excelWorker.GetWorkSheet(1);
                    InputRows.Clear();
                    _excelWorker.ExctractData<SapPersonExcelRow>(_workSheet.TableName).ForEach(_ => InputRows.Add(_));
                }
            }
            IsLoading = false;
        }
    }
}
