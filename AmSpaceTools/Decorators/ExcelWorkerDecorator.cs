using AmSpaceModels;
using AmSpaceModels.Idp;
using AmSpaceModels.Performance;
using ExcelWorker;
using ExcelWorker.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Decorators
{
    public class ExcelWorkerDecorator : IExcelWorker
    {
        private IExcelWorker _decoratee;
        private ILog _logger;

        public string FileName { get; private set; }

        public ExcelWorkerDecorator(IExcelWorker decoratee, ILog logger)
        {
            _decoratee = decoratee;
            _logger = logger;
        }
        public IEnumerable<IdpColumn> GetColumnDataPreview(int rowLimit)
        {
            _logger.Info($"Getting IDP preview from file {FileName} with maximum {rowLimit} rows");
            try
            {
                return _decoratee.GetColumnDataPreview(rowLimit);
            }
            catch (Exception ex)
            {
                _logger.Error("Error during getting IDP preview", ex);
                throw;
            }
        }

        public void SaveData<T>(string fileName, IEnumerable<T> data, string sheetName) where T : class
        {
            _logger.Info($"Saving file {fileName}");
            try
            {
                _decoratee.SaveData(fileName, data, sheetName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during saving file {fileName}", ex);
                throw;
            }
        }

        public IEnumerable<IdpExcelRow> GetAllRows(IEnumerable<IdpColumn> columnDefinitions, bool ignoreFirstRow = true)
        {
            _logger.Info($"Reading all rows from {FileName}");
            try
            {
                return _decoratee.GetAllRows(columnDefinitions, ignoreFirstRow);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during reading rows from {FileName}", ex);
                throw;
            }
        }

        public IEnumerable<T> ExctractData<T>(string sheetName) where T : class, new()
        {
            _logger.Info($"Extracting {(typeof (T)).Name} from {FileName}");
            try
            {
                return _decoratee.ExctractData<T>(sheetName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during extracting {(typeof(T)).Name} from {FileName}", ex);
                throw;
            }
        }

        public IEnumerable<string> GetWorksheets()
        {
            _logger.Info($"Get list of worksheets from file {FileName}");
            try
            {
                return _decoratee.GetWorksheets();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during reading worksheets in file {FileName}", ex);
                throw;
            }
        }

        public DataTable GetWorkSheet(string sheetName)
        {
            _logger.Info($"Get worksheet {sheetName} from file {FileName}");
            try
            {
                return _decoratee.GetWorkSheet(sheetName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during reading worksheet {sheetName} in file {FileName}", ex);
                throw;
            }
        }

        public void OpenFile(string fileName)
        {
            FileName = fileName;
            _logger.Info($"Opening the file {FileName}");
            try
            {
                _decoratee.OpenFile(FileName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during opening the file {FileName}", ex);
                throw;
            }

        }

        public void Dispose()
        {
            _logger.Info($"Disposing excel worker for {FileName}");
            _decoratee.Dispose();
        }

        public DataTable GetWorkSheet(int index)
        {
            _logger.Info($"Get worksheet by index [{index}] from file {FileName}");
            try
            {
                return _decoratee.GetWorkSheet(index);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during reading worksheet by index [{index}] in file {FileName}", ex);
                throw;
            }
        }

        public Task SaveDataAsync<T>(string fileName, IEnumerable<T> data, string sheetName) where T : class
        {
            _logger.Info($"Saving file {fileName}");
            try
            {
                return _decoratee.SaveDataAsync(fileName, data, sheetName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during saving file {fileName}", ex);
                throw;
            }
        }

        public Task<IEnumerable<T>> ExctractDataAsync<T>(string sheetName) where T : class, new()
        {
            _logger.Info($"Extracting {(typeof(T)).Name} from {FileName}");
            try
            {
                return _decoratee.ExctractDataAsync<T>(sheetName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during extracting {(typeof(T)).Name} from {FileName}", ex);
                throw;
            }
        }

        public Task<IEnumerable<IdpExcelRow>> GetAllRowsAsync(IEnumerable<IdpColumn> columnDefinitions, bool ignoreFirstRow = true)
        {
            _logger.Info($"Reading all rows from {FileName}");
            try
            {
                return _decoratee.GetAllRowsAsync(columnDefinitions, ignoreFirstRow);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during reading rows from {FileName}", ex);
                throw;
            }
        }
    }
}
