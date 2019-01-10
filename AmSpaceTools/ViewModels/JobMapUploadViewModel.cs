using AmSpaceClient;
using AmSpaceModels.JobMap;
using AmSpaceModels.Organization;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Infrastructure.Extensions;
using AmSpaceTools.Properties;
using ExcelWorker;
using ExcelWorker.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    class JobMapUploadViewModel : BaseViewModel, IProgressReporter
    {
        private readonly IAmSpaceClient _client;
        private readonly IExcelWorker _excelWorker;
        private string _fileName;
        private DataTable _workSheet;
        private ObservableCollection<JobMapExcelRow> _inputRows;
        private List<JobMapExcelRow> _jobMapsWithErrors;
        public int _newReportsCount;
        private Task<IEnumerable<Brand>> _brands;
        private Task<IEnumerable<Level>> _levels; 
        public ProgressIndicatorViewModel ProgressVM { get; private set; }
        public ObservableCollection<JobMapExcelRow> InputRows
        {
            get => _inputRows;
            set
            {
                _inputRows = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUploadVisible));
            }
        }
        public int NewReportsCount
        {
            get => _newReportsCount;
            set
            {
                if (value >= 0)
                {
                    _newReportsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand OpenFileCommand { get; set; }
        public ICommand OpenReportFolder { get; set; }

        public ICommand UploadDataCommand { get; set; }

        public bool IsUploadVisible => InputRows.Any();

        public JobMapUploadViewModel(IAmSpaceClient client, IExcelWorker excelWorker, ProgressIndicatorViewModel progressVm)
        {
            _client = client;
            _excelWorker = excelWorker;
            ProgressVM = progressVm;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
            OpenReportFolder = new RelayCommand(OpenReportFolderWPF);
            InputRows = new ObservableCollection<JobMapExcelRow>();
            InputRows.CollectionChanged += InputRows_CollectionChanged;
            _jobMapsWithErrors = new List<JobMapExcelRow>();
            _brands = _client.GetBrandsAsync();
            _levels = _client.GetLevelsAsync();
        }

        private void InputRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsUploadVisible));
        }

        private async void UploadData(object obj)
        {
            int doneTasks = 0;
            ProgressVM.ShowLoading();
            ProgressVM.ReportProgress(new ProgressState { ProgressTasksTotal = InputRows.Count, ProgressTasksDone = doneTasks, ProgressDescriptionText = Resources.JobMapInitiatingUploading });
            var groupedResponsibilities = InputRows.GroupBy(item => (item.Country, item.Brand, item.Level, item.Position, item.OrganizationUnit));
            foreach (var collection in groupedResponsibilities)
            {
                if (ProgressVM.IsProgressCancelled)
                    break;
                ProgressVM.ReportProgress(new ProgressState { ProgressTasksTotal = InputRows.Count, ProgressTasksDone = doneTasks, ProgressDescriptionText = string.Format(Resources.JobMapCheckValidity, collection.Key.Position)});
                if (!ValidateJobMapExcelRowCollection(collection, ref doneTasks))
                    continue;
                var foundJobMaps = await GetJobMaps(collection);
                if (!ValidateFoundJobMaps(foundJobMaps, collection, ref doneTasks))
                    continue;
                var jobMap = foundJobMaps.First();
                ProgressVM.ReportProgress(new ProgressState { ProgressTasksTotal = InputRows.Count, ProgressTasksDone = doneTasks, ProgressDescriptionText = string.Format(Resources.JobMapUploading, jobMap.JobDescriptionsTranslations.First().Name) });
                var jobDescription = BuildJobDescription(collection.First(), jobMap.Id);
                await UpdateJobDescriptions(jobDescription);
                await CleanJobResponsibilities(jobMap);
                var responsibilitiesForUploading = collection.Reverse().Select(item => BuildJobResponsibility(item, jobMap.Id));
                foreach (var record in responsibilitiesForUploading)
                {
                    if (ProgressVM.IsProgressCancelled)
                        break;
                    ProgressVM.ReportProgress(new ProgressState { ProgressTasksTotal = InputRows.Count, ProgressTasksDone = doneTasks++, ProgressDescriptionText = string.Format(Resources.JobMapUploading, jobMap.JobDescriptionsTranslations.First().Name) });
                    await UploadJobResponsibility(record);
                }
            }
            ProgressVM.ReportProgress(new ProgressState { ProgressTasksTotal = InputRows.Count, ProgressTasksDone = doneTasks, ProgressDescriptionText = Resources.JobMapUploadComplete });
            if (_jobMapsWithErrors.Any())
            {
                ProgressVM.ReportProgress(new ProgressState { ProgressTasksTotal = InputRows.Count, ProgressTasksDone = doneTasks, ProgressDescriptionText = Resources.JobMapUploadCompleteWithIssues });
                await SaveErrorsToFile();
            }
        }

        private async void OpenFile(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Multiselect = false
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                IsLoading = true;
                using (_excelWorker)
                {
                    _fileName = dialog.FileName;
                    await _excelWorker.OpenFileAsync(_fileName);
                    _workSheet = await _excelWorker. GetWorkSheetAsync(1);
                    var data = await _excelWorker.ExctractDataAsync<JobMapExcelRow>(_workSheet.TableName);
                    InputRows.Clear();
                    data.ForEach(row => InputRows.Add(row));
                }
            }
            IsLoading = false;
        }

        private async Task<IEnumerable<JobMap>> GetJobMaps(IGrouping<(string, string, int, string, string), JobMapExcelRow> collection)
        {
            var (stringCountry, stringBrand, intLevel, position, department) = collection.Key;
            var (country, level) = await GetSearchObjects(stringCountry, stringBrand, intLevel, await _brands, await _levels);
            return await FindJobMap(country, level, position, department);
        }

        private async Task<(Country, Level)> GetSearchObjects(string country, string brand, int level, IEnumerable<Brand> brands, IEnumerable<Level> levels)
        {
            var currentBrand = brands.FirstOrDefault(item => item.Name == brand);
            var currentLevel = levels.FirstOrDefault(item => item.Name == level.ToString());
            Country currentCountry = null;
            if (currentBrand != null)
            {
                var countries = await _client.GetCountriesAsync(currentBrand);
                currentCountry = countries.FirstOrDefault(item => item.Name.Contains(country));
            }
            return (currentCountry, currentLevel);
        }

        private async Task SaveErrorsToFile()
        {
            using (_excelWorker)
            {
                await _excelWorker.SaveDataAsync($"{DateTime.Now.ToShortDateString()}_JobMapUploadErrors_{DateTime.Now.Millisecond}.xlsx", AmSpaceModels.Enums.AppDataFolders.Reports, _jobMapsWithErrors, "Errors");
            }
            NewReportsCount++;
            _jobMapsWithErrors.Clear();
        }

        private async Task UpdateJobDescriptions(JobDescription jobDescription)
        {
            await _client.UpdateJobDescriptionAsync(jobDescription);
        }

        private async Task UploadJobResponsibility(JobResponsibility jobResponsibility)
        {
            await _client.CreateJobResponsibilityAsync(jobResponsibility);
        }

        private JobDescription BuildJobDescription(JobMapExcelRow jobMapExcelRow, long? id)
        {
            var translations = new List<JobDescriptionTranslation>();
            translations.Add(new JobDescriptionTranslation
            {
                Name = jobMapExcelRow.Position,
                Description = jobMapExcelRow.JobPurposeEng,
                Language = "en"

            });
            if (!string.IsNullOrEmpty(jobMapExcelRow.Country))
            {
                translations.Add(new JobDescriptionTranslation
                {
                    Name = jobMapExcelRow.Position,
                    Description = jobMapExcelRow.JobPurposeLocal,
                    Language = TranslationsMap.Map[jobMapExcelRow.Country.Trim()]
                });
            }
            return new JobDescription { Id = id, Translations = translations };
        }

        private JobResponsibility BuildJobResponsibility(JobMapExcelRow row, long? jobId)
        {
            var translations = new List<ResponsibilityTranslation>();
            translations.Add(new ResponsibilityTranslation
            {
                Description = row.ResponsibilityEng,
                KpiText = row.KPIEng,
                Language = "en"
            });
            if (!string.IsNullOrEmpty(row.Country))
            {
                translations.Add(new ResponsibilityTranslation
                {
                    Description = row.ResponsibilityLocal,
                    KpiText = row.KPILocal,
                    Language = TranslationsMap.Map[row.Country.Trim()]
                });
            }
            return new JobResponsibility { Translations = translations, Job = jobId.Value };
        }

        private async Task CleanJobResponsibilities(JobMap jobMap)
        {
            var list = await _client.GetJobResponsibilitiesAsync(jobMap);
            foreach (var responsibility in list)
            {
                await _client.DeleteJobResponsibilityAsync(responsibility);
            }
        }

        private async Task<IEnumerable<JobMap>> FindJobMap(Country country, Level level, string position, string department)
        {
            var searchResult = await _client.FindJobMapAsync(country, level, position);
            var resutl = searchResult.Where(element => element.JobDescriptionsTranslations.Any(item => item.Name == position.Trim())
                                                                                        && (element.Department == department.Trim() || element.Country == department.Trim()));
            return resutl;
        }

        private bool ValidateFoundJobMaps(IEnumerable<JobMap> foundJobMaps, IEnumerable<JobMapExcelRow> responsibilitiesCollection, ref int taskCounter)
        {
            var result = true;
            if (foundJobMaps.Count() != 1)
            {
                taskCounter += responsibilitiesCollection.Count();
                _jobMapsWithErrors.AddRange(responsibilitiesCollection);
                result = false;
            }
            return result;
        }
        private bool ValidateJobMapExcelRowCollection(IEnumerable<JobMapExcelRow> collection, ref int taskCounter)
        {
            var result = true;
            var incorrectLines = collection.Where(item => ValidateEntry(item));
            if (incorrectLines.Any())
            {
                taskCounter += collection.Count();
                _jobMapsWithErrors.AddRange(collection);
                result = false;
            }
            return result;
        }

        private bool ValidateEntry(JobMapExcelRow row)
        {
            return string.IsNullOrEmpty(row?.Brand) ||
                string.IsNullOrEmpty(row?.Position) ||
                string.IsNullOrEmpty(row?.JobPurposeEng) ||
                string.IsNullOrEmpty(row?.ResponsibilityEng) ||
                string.IsNullOrEmpty(row?.KPIEng);
        }

        private void OpenReportFolderWPF(object ojb)
        {
            Process.Start(AmSpaceModels.Enums.FoldersLocations.GetFolderLocation(AmSpaceModels.Enums.AppDataFolders.Reports));
            NewReportsCount = 0;
        }
    }
}
