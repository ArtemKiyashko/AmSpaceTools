using AmSpaceTools.Infrastructure;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class ErrorViewModel : BaseViewModel
    {
        private string _errorMsg;
        public string ErrorMsg { get => _errorMsg; set => _errorMsg = value; }

        public ErrorViewModel(string errorMsg)
        {
            _errorMsg = errorMsg ;
        }
    }
}
