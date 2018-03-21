using AmSpaceModels;
using AmSpaceTools.Infrastructure;
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
    public class IdpTranslationsPreviewViewModel : BaseViewModel
    {
        private IEnumerable<IdpExcelColumn> _excelColumnsPreview;
        private IExcelWorker _excelWorker;
        private ICommand _openFileCommand;
        private IEnumerable<IdpExcelColumn> _allColumns;
        private ICommand _uploadDataCommand;

        public IEnumerable<IdpExcelColumn> ExcelColumnsPreview
        {
            get
            {
                if (_excelColumnsPreview == null) _excelColumnsPreview = new List<IdpExcelColumn>();
                return _excelColumnsPreview;
            }
            set
            {
                _excelColumnsPreview = value;
                OnPropertyChanged(nameof(ExcelColumnsPreview));
            }
        }

        public IdpTranslationsPreviewViewModel(IExcelWorker excelWorker)
        {
            _excelWorker = excelWorker;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
        }

        private void UploadData(object obj)
        {
            throw new NotImplementedException();
        }

        public ICommand UploadDataCommand
        {
            get { return _uploadDataCommand; }
            set { _uploadDataCommand = value; }
        }
        public ICommand OpenFileCommand
        {
            get { return _openFileCommand; }
            set { _openFileCommand = value; }
        }

        private void OpenFile(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                _allColumns = _excelWorker.GetData(dialog.FileName);
                ShowPreview(_allColumns);
            }
        }

        private void ShowPreview(IEnumerable<IdpExcelColumn> source)
        {
            IsLoading = true;
            var firstWorksheet = _allColumns
               .Where(_ => _.WorkSheet == _allColumns.Select(w => w.WorkSheet).Min()).ToList();
            var resultPreview = new List<IdpExcelColumn>();
            foreach (var c in firstWorksheet)
            {
                var preview = new IdpExcelColumn();
                preview.ColumntAddress = c.ColumntAddress;
                preview.ColumnType = c.ColumnType;
                preview.WorkSheet = c.WorkSheet;
                preview.ColumnData = c.ColumnData.Take(5);
                resultPreview.Add(preview);
            }
            ExcelColumnsPreview = resultPreview;
            IsLoading = false;
        }
    }
}
