using AmSpaceClient;
using AmSpaceTools.Infrastructure;
using AutoMapper;
using ExcelWorker;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

        public IEnumerable<string> WorkSheets { get; set; }
        public ICommand OpenFileCommand { get; set; }



        public PeopleUploadViewModel(IAmSpaceClient client, IMapper mapper, IExcelWorker excelWorker)
        {
            _client = client;
            _mapper = mapper;
            _excelWorker = excelWorker;
            OpenFileCommand = new RelayCommand(OpenFile);
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
                WorkSheets = _excelWorker.GetWroksheets();
            }
            IsLoading = false;
        }
    }
}
