using AmSpaceClient;
using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using AutoMapper;
using ExcelWorker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class OrgStructureViewModel : BaseViewModel
    {
        public IEnumerable<AmspaceDomain> DomainTree
        {
            get
            {
                return _domainTree;
            }
            set
            {
                _domainTree = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StructureIsEmpty));
            }
        }

        public ICommand EditDomainNameCommand { get => _editDomainNameCommand; set => _editDomainNameCommand = value; }
        public ICommand DeleteDomainCommand { get => _deleteDomainCommand; set => _deleteDomainCommand = value; }
        public ICommand CreateDomainCommand { get => _createDomainCommand; set => _createDomainCommand = value; }
        public ICommand GetDomainTreeFromServerCommand { get => _getDomainTreeFromServerCommand; set => _getDomainTreeFromServerCommand = value; }
        public bool StructureIsEmpty { get { return DomainTree == null || !DomainTree.Any(); } }

        public ICommand OpenFileCommand { get => _openFileCommand; set => _openFileCommand = value; }

        private ICommand _editDomainNameCommand;
        private ICommand _deleteDomainCommand;
        private ICommand _createDomainCommand;
        private ICommand _getDomainTreeFromServerCommand;
        private ICommand _openFileCommand;
        private IEnumerable<AmspaceDomain> _domainTree;
        private readonly IExcelWorker _excelWorker;
        private readonly IMapper _mapper;
        private readonly IAmSpaceClient _client;

        public OrgStructureViewModel(IExcelWorker excelWorker, IMapper mapper, IAmSpaceClient client)
        {
            _excelWorker = excelWorker;
            _mapper = mapper;
            _client = client;
            GetDomainTreeFromServerCommand = new RelayCommand(DownloadTree);
            OpenFileCommand = new RelayCommand(OpenTreeFromFile);
            CreateDomainCommand = new RelayCommand(CreateDomain);
            EditDomainNameCommand = new RelayCommand(EditDomainName);
            DeleteDomainCommand = new RelayCommand(DeleteDomain);
        }

        private void DeleteDomain(object obj)
        {
            throw new NotImplementedException();
        }

        private void CreateDomain(object obj)
        {
            throw new NotImplementedException();
        }

        private void EditDomainName(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenTreeFromFile(object obj)
        {
            throw new NotImplementedException();
        }

        private async void DownloadTree(object obj)
        {
            IsLoading = true;
            DomainTree = await _client.GetOrganizationStructureAsync();
            IsLoading = false;
        }
    }
}
