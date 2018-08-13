﻿using AmSpaceModels.Idp;
using AmSpaceModels.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker
{
    public interface IExcelWorker
    {
        IEnumerable<IdpExcelColumn> GetColumnDataPreview(string fileName, int rowLimit);
        IEnumerable<IdpExcelRow> GetAllRows(string fileName, IEnumerable<IdpExcelColumn> columnDefinitions, bool ignoreFirstRow = true);
        void SaveData<T>(string fileName, IEnumerable<T> data, string sheetName) where T : class;
        IEnumerable<KpiExcelRow> ExctractKpiData(string fileName, string sheetName);
        IEnumerable<GoalExcelRow> ExctractGoalData(string fileName, string sheetName);
    }
}
