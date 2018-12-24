using AmSpaceClient;
using AmSpaceTools.Infrastructure;
using ExcelWorker;
using ExcelWorker.Models;
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
    class JobMapUploadViewModel : BaseViewModel, IProgressReporter
    {
        private readonly IAmSpaceClient _client;
        private readonly IExcelWorker _excelWorker;
        private string _fileName;
        private DataTable _workSheet;
        private ObservableCollection<JobMapExcelRow> _inputRows;
        public ProgressIndicatorViewModel ProgressVM { get; private set; }
        public ObservableCollection<JobMapExcelRow> InputRows
        {
            get => _inputRows;
            set
            {
                _inputRows = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUploadVisible));
            }
        }
        public ICommand OpenFileCommand { get; set; }

        public ICommand UploadDataCommand { get; set; }

        public bool IsUploadVisible => InputRows.Any();

        public JobMapUploadViewModel(IAmSpaceClient client, IExcelWorker excelWorker, ProgressIndicatorViewModel progressVm)
        {
            _client = client;
            _excelWorker = excelWorker;
            ProgressVM = progressVm;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
            InputRows = new ObservableCollection<JobMapExcelRow>();
        }

        private void InputRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsUploadVisible));
        }

        private async void UploadData(object obj)
        {

        }

        private async void OpenFile(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Multiselect = false
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                IsLoading = true;
                using (_excelWorker)
                {
                    _fileName = dialog.FileName;
                    await _excelWorker.OpenFileAsync(_fileName);
                    _workSheet = await _excelWorker. GetWorkSheetAsync(1);
                    var data = await _excelWorker.ExctractDataAsync<JobMapExcelRow>(_workSheet.TableName);
                    InputRows.Clear();
                    data.ForEach(row => InputRows.Add(row));
                }
            }
            IsLoading = false;
        }
    }
}
