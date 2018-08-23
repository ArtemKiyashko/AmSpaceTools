﻿using AmSpaceModels;
using AmSpaceModels.Idp;
using AmSpaceModels.Performance;
using ExcelWorker;
using ExcelWorker.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Decorators
{
    public class ExcelWorkerDecorator : IExcelWorker
    {
        private IExcelWorker _decoratee;
        private ILog _logger;

        public ExcelWorkerDecorator(IExcelWorker decoratee, ILog logger)
        {
            _decoratee = decoratee;
            _logger = logger;
        }
        public IEnumerable<IdpColumn> GetColumnDataPreview(string fileName, int rowLimit)
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

        public void SaveData<T>(string fileName, IEnumerable<T> data, string sheetName) where T : class
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

        public IEnumerable<IdpExcelRow> GetAllRows(string fileName, IEnumerable<IdpColumn> columnDefinitions, bool ignoreFirstRow = true)
        {
            _logger.Info($"Reading all rows from {fileName}");
            try
            {
                return _decoratee.GetAllRows(fileName, columnDefinitions, ignoreFirstRow);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during reading rows from {fileName}", ex);
                throw;
            }
        }

        public IEnumerable<T> ExctractData<T>(string fileName, string sheetName) where T : class, new()
        {
            _logger.Info($"Extracting {(typeof (T)).Name} from {fileName}");
            try
            {
                return _decoratee.ExctractData<T>(fileName, sheetName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during extracting {(typeof(T)).Name} from {fileName}", ex);
                throw;
            }
        }
    }
}
