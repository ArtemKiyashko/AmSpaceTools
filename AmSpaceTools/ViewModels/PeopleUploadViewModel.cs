using AmSpaceClient;
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
            var inputSapUsers = _mapper.Map<IEnumerable<SapUser>>(InputRows);
            var inputRowsGroupedByContracts = InputRows.GroupBy(_ => new { _.IdentityNumber, _.ManagerId }, v => v);
            var tree = inputRowsGroupedByContracts.GenerateTree(c => c.Key.IdentityNumber, c => c.Key.ManagerId ?? string.Empty, string.Empty);
            foreach (var account in tree.Descendants(_ => _.Children))
            {
                foreach(var contract in account.Item)
                {
                    //TODO: convert to temp account model (using auto mapper). upload to AmSpace
                    throw new NotImplementedException();
                }
            }
            //below code useless
            foreach(var inputRow in InputRows.Where(_ => string.IsNullOrEmpty(_.ManagerId)).GroupBy(_ => _.IdentityNumber))
            {
                foreach (var inputContract in inputRow)
                {
                    var currentUserDomainName = flatDomains.FirstOrDefault(_ => _.Id == inputContract.Mpk);
                    if (currentUserDomainName == null) throw new Exception($"Cannot find domain with MPK {inputContract.Mpk} for User {inputContract.Name} {inputContract.Surname}");
                    var existingUsersInDomain = await _client.FindUser($"{inputContract.Name} {inputContract.Surname}", null, null, UserStatus.ANY, currentUserDomainName.Name);
                    if (existingUsersInDomain.Count() > 1) throw new Exception($"More than one {inputContract.Name} {inputContract.Surname} in domain {inputContract.Mpk}");
                    //var sapUser = inputSapUsers.Single(_ => _.PersonLegalId == inputContract.IdentityNumber);
                    //sapUser.MainEmployeeId = inputRow.Single(_ => _.ContractNumber == 1).EmployeeId;
                }
            }
            IsLoading = false;
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
