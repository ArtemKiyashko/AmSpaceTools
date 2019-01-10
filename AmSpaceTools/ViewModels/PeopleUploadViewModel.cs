using AmSpaceClient;
using AmSpaceModels.Enums;
using AmSpaceModels.Organization;
using AmSpaceModels.Sap;
using AmSpaceModels.Auth;
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AmSpaceTools.Properties;
using AmSpaceTools.Infrastructure.Extensions;
using AmSpaceTools.Infrastructure.Providers;
using System.Windows.Threading;

namespace AmSpaceTools.ViewModels
{
    public class PeopleUploadViewModel : BaseViewModel, IProgressReporter
    {
        private readonly IAmSpaceClient _client;
        private readonly IMapper _mapper;
        private readonly IExcelWorker _excelWorker;
        private string _fileName;
        private DataTable _workSheet;
        private ObservableCollection<SapPersonExcelRow> _inputRows;
        private readonly SearchPeopleViewModel _searchVm;
        private readonly ChangePasswordViewModel _changePasswordVm;
        private Func<SapPersonExcelRow, bool> _defaultPasswordRequiredCondition => a => string.IsNullOrEmpty(a.Email) && a.Level < 5;
        private NewPassword _defaultPassword;
        private readonly IActiveDirectoryProvider _activeDirectoryProvider;
        private bool _adConnected;

        public bool AdConnected
        {
            get
            {
                return _adConnected;
            }
            set
            {
                _adConnected = value;
                OnPropertyChanged();
            }
        }

        public ProgressIndicatorViewModel ProgressVM { get; private set; }

        public ObservableCollection<SapPersonExcelRow> InputRows
        {
            get { return _inputRows; }
            set { _inputRows = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsUploadVisible)); }
        }
        public ICommand OpenFileCommand { get; set; }

        public ICommand UploadDataCommand { get; set; }

        public bool IsUploadVisible => InputRows.Any();
        
        public PeopleUploadViewModel(
            IAmSpaceClient client,
            IMapper mapper,
            IExcelWorker excelWorker,
            SearchPeopleViewModel searchVm,
            ProgressIndicatorViewModel progressVm,
            ChangePasswordViewModel changePasswordVm,
            IActiveDirectoryProvider activeDirectoryProvider)
        {
            ProgressVM = progressVm;
            _client = client;
            _mapper = mapper;
            _excelWorker = excelWorker;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
            _searchVm = searchVm;
            InputRows = new ObservableCollection<SapPersonExcelRow>();
            InputRows.CollectionChanged += InputRows_CollectionChanged;
            _changePasswordVm = changePasswordVm;
            _activeDirectoryProvider = activeDirectoryProvider;
            _activeDirectoryProvider.ConnectionStatusChanged += _activeDirectoryProvider_ConnectionStatusChanged;
        }

        private void _activeDirectoryProvider_ConnectionStatusChanged(object sender, ConnectionStatusEventArgs e)
        {
            AdConnected = e.IsConnected;
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
                throw new ArgumentException($"{Resources.PeopleUploadMissingMpkMessage} {missingMpks.Select(_ => _.ToString()).Aggregate((c, n) => c + ", " + n)}", nameof(SapPersonExcelRow.Mpk));
        }

        private async Task RequestDefaultPassword(IEnumerable<SapPersonExcelRow> inputRows)
        {
            if (inputRows.Any(_defaultPasswordRequiredCondition))
            {
                _changePasswordVm.DescriptionText = Resources.PeopleUploadSetDefaultPassword;
                var view = new ChangePassword()
                {
                    DataContext = _changePasswordVm
                };
                var result = (bool)await DialogHost.Show(view, "ControlDialog");
                if (!result) throw new ArgumentNullException(Resources.PeopleUploadDefaultPasswordNotSet);
                _defaultPassword = _changePasswordVm.Password;
            }
        }
        private async void UploadData(object obj)
        {
            ProgressVM.ShowLoading();
            ProgressVM.ReportProgress(new ProgressState { ProgressTasksDone = 0, ProgressDescriptionText = Resources.PeopleUploadValidateDomainMessage });
            await ValidateDomainsAsync(InputRows);
            if (ProgressVM.IsProgressCancelled) return;
            var inputRowsGroupedByContracts = InputRows.GroupBy(_ => new { _.IdentityNumber, _.ManagerId }, v => v);
            ProgressVM.ReportProgress(new ProgressState { ProgressTasksDone = 0, ProgressDescriptionText = Resources.PeopleUploadGenerateStructureTreeMessage });
            var tree = inputRowsGroupedByContracts.GenerateTree(c => c.Key.IdentityNumber, c => c.Key.ManagerId ?? string.Empty, string.Empty);
            await RequestDefaultPassword(InputRows);
            if (ProgressVM.IsProgressCancelled) return;
            var descendantList = tree.Descendants(_ => _.Children).ToList();
            var contactsCount = descendantList.SelectMany(_ => _.Item).Count();
            var uploadedContracts = 0;
            foreach (var account in descendantList)
            {
                if (ProgressVM.IsProgressCancelled) break;
                foreach (var contract in account.Item)
                {
                    var externalAccount = await FillAccount(contract);
                    var accountResult = await UploadAccount(externalAccount);
                    ProgressVM.ReportProgress(new ProgressState { ProgressTasksDone = ++uploadedContracts, ProgressTasksTotal = contactsCount, ProgressDescriptionText = string.Format(Resources.PeopleUploadReportUploadStatusMessage, contract.Name, contract.Surname, contract.IdentityNumber) });
                }
            }
            ProgressVM.ReportProgress(new ProgressState { ProgressTasksDone = uploadedContracts, ProgressTasksTotal = contactsCount, ProgressDescriptionText = string.Format(Resources.PeopleUploadFinishUploadMessage, Environment.NewLine, uploadedContracts) });
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
                var result = (bool)await DialogHost.Show(view, "ControlDialog");
                if (!result) throw new ArgumentNullException(nameof(contract.ManagerId), string.Format(Resources.PeopleUploadManagerNotSetMessage, contract.Name, contract.Surname));
                externalAccount.ManagerId = _searchVm.SelectedUser.User.Id;
            }
            else
            {
                var manager = await _client.FindUserByIdentityNumber(contract.ManagerId);
                externalAccount.ManagerId = manager == null ? throw new ArgumentException(string.Format(Resources.PeopleUploadManagerNotFoundMessage, contract.Name, contract.Surname)) : manager.Id;
            }
        }

        protected async Task<ExternalAccountResponse> UploadAccount(ExternalAccount externalAccount)
        {
            var contractResult = new ExternalAccountResponse();
            var existingUser = await _client.FindUserByIdentityNumber(externalAccount.PersonLegalId);
            if (existingUser == null)
                contractResult = await _client.CreateExternalAccount(externalAccount);
            else
            {
                var existingContract = existingUser.Contracts.Find(_ => _.ContractNumber == externalAccount.ContractNumber);
                contractResult = existingContract == null ?
                    await _client.CreateExternalAccount(externalAccount) :
                    await _client.UpdateExternalAccount(existingContract.Id, externalAccount);
            }
            var passwordChanged = await ChangePassword(externalAccount, InputRows);
            return contractResult;
        }

        protected async Task<bool> ChangePassword(ExternalAccount account, IEnumerable<SapPersonExcelRow> inputRows)
        {
            var row = inputRows
                .Where(_ => _.IdentityNumber == account.PersonLegalId && 
                _.ContractNumber == account.ContractNumber && 
                _.Status == ContractStatus.ACTIVE)
                .SingleOrDefault(_defaultPasswordRequiredCondition);
            if (account.BackendType == AccountBackendType.ActiveDirectory || row == null) return false;
            var existingUser = await _client.FindUserByIdentityNumber(account.PersonLegalId);
            bool result = false;
            try
            {
                result = await _client.ChangePasswordAsync(_defaultPassword, existingUser);
            }
            catch (ArgumentException ex)
            {
                if (!ex.Message.Contains("New password cannot be the same as old password.")) throw;
            }
            return result;
        }

        private async void OpenFile(object obj)
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
                    await _excelWorker.OpenFileAsync(_fileName);
                    _workSheet = await _excelWorker.GetWorkSheetAsync(1);
                    InputRows.Clear();
                    var data = await _excelWorker.ExctractDataAsync<SapPersonExcelRow>(_workSheet.TableName);
                    data.ForEach(_ => InputRows.Add(_));
                }
            }
            IsLoading = false;
        }
    }
}
