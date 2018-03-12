using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            SelectedViewModel = new LoginViewModel(this);
        }
    }
}
