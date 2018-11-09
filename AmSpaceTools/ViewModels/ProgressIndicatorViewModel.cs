using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AmSpaceTools.Infrastructure;

namespace AmSpaceTools.ViewModels
{
    public class ProgressIndicatorViewModel : BaseViewModel
    {
        private IProgressState _progress;
        public ICommand CancelCommand { get; set; }
        public event EventHandler OnCancelButtonClick;
        public IProgressState Progress
        {
            get
            {
                return _progress;
            }
            private set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
        public ProgressIndicatorViewModel()
        {
            _progress = default;
            CancelCommand = new RelayCommand(CancelProcess);
        }

        public void ReportProgress(IProgressState value)
        {
            Progress = value;
        }

        public void CancelProcess(object tokenSource)
        {
            OnCancelButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
