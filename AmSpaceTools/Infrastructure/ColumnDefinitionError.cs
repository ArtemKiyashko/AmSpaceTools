using AmSpaceModels;
using AmSpaceModels.Idp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public class ColumnDefinitionError
    {
        public ColumnActionType ColumnType { get; private set; }
        public string ErrorMsg { get { return $"Column for action {ColumnType} not defined!"; } }

        public ColumnDefinitionError(ColumnActionType columnType)
        {
            ColumnType = columnType;
        }
    }
}
