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

        public ICommand EditDomainName { get => _editDomainName; set => _editDomainName = value; }
        public ICommand DeleteDomain { get => _deleteDomain; set => _deleteDomain = value; }
        public ICommand CreateDomain { get => _createDomain; set => _createDomain = value; }
        public ICommand GetDomainTreeFromServer { get => _getDomainTreeFromServer; set => _getDomainTreeFromServer = value; }
        public bool StructureIsEmpty { get { return DomainTree == null || !DomainTree.Any(); } }

        public ICommand OpenFileCommand { get => _openFileCommand; set => _openFileCommand = value; }

        private ICommand _editDomainName;
        private ICommand _deleteDomain;
        private ICommand _createDomain;
        private ICommand _getDomainTreeFromServer;
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
            GetDomainTreeFromServer = new RelayCommand(DownloadTree);
            OpenFileCommand = new RelayCommand(OpenTreeFromFile);
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
