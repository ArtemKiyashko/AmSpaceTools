using AmSpaceTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public abstract class ProgressBaseViewModel : BaseViewModel
    {
        private ProgressIndicatorViewModel _progressVM;
        protected IProgress<IProgressState> _progressReporter;
        protected CancellationTokenSource _cancellationTokenSource;

        public ProgressIndicatorViewModel ProgressVM
        {
            get => _progressVM;
            private set
            {
                _progressVM = value;
                OnPropertyChanged();
            }
        }

        public ProgressBaseViewModel()
        {
            ProgressVM = Services.Container.GetInstance<ProgressIndicatorViewModel>();
            InitializeProgressBarReports();
        }

        private void InitializeProgressBarReports()
        {
            _progressReporter = new Progress<IProgressState>(ProgressVM.ReportProgress);
            _cancellationTokenSource = new CancellationTokenSource();
            _progressVM.OnCancelButtonClick += (obj, handler) => _cancellationTokenSource.Cancel();
        }
    }
}
