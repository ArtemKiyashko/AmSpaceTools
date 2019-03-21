using AmSpaceClient;
using AmSpaceModels.Organization;
using AmSpaceModels.Sap;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Infrastructure.Extensions;
using AmSpaceTools.Views;
using AutoMapper;
using ExcelWorker;
using MaterialDesignThemes.Wpf;
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

        private async void DeleteDomain(object obj)
        {
            var domain = (AmspaceDomain)obj;
            var view = new ConfirmUnitDelete()
            {
                DataContext = domain
            };
            var result = (bool)await DialogHost.Show(view, "RootDialog");
            if (!result) return;
            IsLoading = true;
            var deleteResult = await _client.PutDomainAsync(new SapDomain {
                DomainId = domain.Id,
                Name = domain.Name,
                Mpk = -1,
                Status = false,
                ParentDomainId = 0
            });
            if (deleteResult) DownloadTree(null);
        }

        private async void CreateDomain(object obj)
        {
            var parentDomain = (AmspaceDomain)obj;
            var newDomain = new SapDomain();
            var view = new EditUnitName()
            {
                DataContext = newDomain
            };
            var result = (bool)await DialogHost.Show(view, "RootDialog");
            if (!result) return;
            newDomain.DomainId = DomainTree.Descendants(_ => _.Children).Max(_ => _.Id) + 1;
            newDomain.ParentDomainId = parentDomain.Id;
            newDomain.Mpk = -1;
            newDomain.Status = true;
            IsLoading = true;
            var createResult = await _client.PutDomainAsync(newDomain);
            if (createResult) DownloadTree(null);
        }

        private async void EditDomainName(object obj)
        {
            var domain = (AmspaceDomain)obj;
            var copy = new AmspaceDomain {Id = domain.Id, Mpk = domain.Mpk, Name = domain.Name, Children = domain.Children};
            var view = new EditUnitName()
            {
                DataContext = copy
            };
            var result = (bool)await DialogHost.Show(view, "RootDialog");
            if (!result) return;
            IsLoading = true;
            var editResult = await _client.PutDomainAsync(new SapDomain
                {
                    DomainId = copy.Id,
                    Name = copy.Name,
                    Mpk = Convert.ToInt32(copy.Mpk.GetValueOrDefault(-1)),
                    Status = true,
                    ParentDomainId = domain.FindParentNode(DomainTree, (_) => _.Children).Id
                });
            if (editResult) DownloadTree(null);
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
