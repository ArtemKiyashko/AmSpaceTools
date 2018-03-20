﻿using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker
{
    public interface IExcelWorker<T> : IExcelWorker
    {
        void SaveData(string fileName, IEnumerable<T> data, string sheetName);
    }

    public interface IExcelWorker
    {
        IEnumerable<IdpExcelColumn> GetColumnDataPreview(string fileName, int rowLimit);
        IEnumerable<ColumnDefinitionError> ValidateColumnDefinitions(IEnumerable<IdpExcelColumn> columnDefinitions);
        IEnumerable<IdpExcelRow> GetAllRows(string fileName, IEnumerable<IdpExcelColumn> columnDefinitions, bool ignoreFirstRow = true);
    }
}
