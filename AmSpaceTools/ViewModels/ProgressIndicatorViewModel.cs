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

        public void ShowLoading(string rootDialogName = null)
        {
            _progress = new ProgressState { ProgressTasksDone = 0 };
            var view = new ProgressIndicator()
            {
                DataContext = this
            };
            _dialogTask = DialogHost.Show(view, string.IsNullOrEmpty(rootDialogName) ? "RootDialog" : rootDialogName);
        }

        public void CloseLoading()
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
        }

        public bool IsProgressCancelled => _dialogTask.IsCompleted ? !(bool)_dialogTask.GetAwaiter().GetResult() : false;

        public void CancelProcess(object tokenSource)
        {
            OnCancelButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
