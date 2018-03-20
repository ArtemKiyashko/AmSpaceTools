﻿using AmSpaceModels;
using ExcelWorker;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Decorators
{
    public class ExcelWorkerDecorator<T> : IExcelWorker<T>
    {
        private IExcelWorker<T> _decoratee;
        private ILog _logger;

        public ExcelWorkerDecorator(IExcelWorker<T> decoratee, ILog logger)
        {
            _decoratee = decoratee;
            _logger = logger;
        }
        public IEnumerable<IdpExcelColumn> GetColumnDataPreview(string fileName, int rowLimit)
        {
            _logger.Info($"Getting IDP preview from file {fileName} with maximum {rowLimit} rows");
            try
            {
                return _decoratee.GetColumnDataPreview(fileName, rowLimit);
            }
            catch (Exception ex)
            {
                _logger.Error("Error during getting IDP preview", ex);
                throw;
            }
        }

        public void SaveData(string fileName, IEnumerable<T> data, string sheetName)
        {
            _logger.Info($"Saving {fileName}");
            try
            {
                _decoratee.SaveData(fileName, data, sheetName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during saving {fileName}", ex);
                throw;
            }
        }

        public IEnumerable<ColumnDefinitionError> ValidateColumnDefinitions(IEnumerable<IdpExcelColumn> columnDefinitions)
        {
            //TODO
            return _decoratee.ValidateColumnDefinitions(columnDefinitions);
        }
    }
}
