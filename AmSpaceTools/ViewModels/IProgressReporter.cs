using AmSpaceTools.Infrastructure;
using AmSpaceTools.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public interface IProgressReporter
    {
        ProgressIndicatorViewModel ProgressVM { get; }
    }
}
