using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using MaterialDesignThemes.Wpf;

namespace AmSpaceTools.ViewModels
{
    public class ProgressIndicatorViewModel : BaseViewModel
    {
        private IProgressState _progress;
        private IProgress<IProgressState> _progressReporter;
        public ICommand CancelCommand { get; set; }
        public event EventHandler OnCancelButtonClick;
        private DialogSession _session;
        private Task<object> _dialogTask;
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
            _progressReporter = new Progress<IProgressState>((value) => Progress = value);
            CancelCommand = new RelayCommand(CancelProcess);
        }

        public void ReportProgress(IProgressState value)
        {
            _progressReporter.Report(value);
        }

        public void ShowLoading(string rootDialogName = "RootDialog")
        {
            _progress = default(ProgressState);
            var view = new ProgressIndicator()
            {
                DataContext = this
            };
            _dialogTask = DialogHost.Show(view, rootDialogName, (sender, session) => _session = session.Session, null);
        }

        public void CloseLoading()
        {
            if(!_session.IsEnded)
                _session.Close(true);
        }

        public bool IsProgressCancelled => _dialogTask.IsCompleted ? !(bool)_dialogTask.GetAwaiter().GetResult() : false;

        public void CancelProcess(object tokenSource)
        {
            OnCancelButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
