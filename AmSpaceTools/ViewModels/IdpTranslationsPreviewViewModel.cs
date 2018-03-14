using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using ExcelWorker;
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
        private ICommand _loadDataCommand;
        private IEnumerable<IdpExcelColumn> _allColumns;

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
            LoadDataCommand = new RelayCommand(LoadData);
        }

        public ICommand LoadDataCommand
        {
            get { return _loadDataCommand; }
            set { _loadDataCommand = value; }
        }

        private void LoadData(object obj)
        {
            _allColumns = _excelWorker.GetData(@"C:\Users\artem.kiyashko\Documents\idp_hr.xlsx");
            //ExcelColumnsPreview = _allColumns.Where(_ => ).Take(5);
        }
    }
}
