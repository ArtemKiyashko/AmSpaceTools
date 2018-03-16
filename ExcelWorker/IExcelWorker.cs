using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker
{
    public interface IExcelWorker
    {
        IEnumerable<IdpExcelColumn> GetData(string fileName);
    }
}
