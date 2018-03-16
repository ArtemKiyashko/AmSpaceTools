using AmSpaceTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            SelectedViewModel = Services.Container.GetInstance<LoginViewModel>();
        }
    }
}
