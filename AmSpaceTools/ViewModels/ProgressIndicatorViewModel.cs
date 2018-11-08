﻿using System;
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
            if (value.ProgressPercent > Progress?.ProgressPercent || value.ProgressStatus != Progress?.ProgressStatus)
            {
                Progress = value;
            }
        }

        public void CancelProcess(object tokenSource)
        {
            (tokenSource as CancellationTokenSource)?.Cancel();
        }
    }
}
