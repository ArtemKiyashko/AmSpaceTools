﻿using AmSpaceClient;
using AmSpaceModels.Idp;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Infrastructure.Extensions;
using AmSpaceTools.ModelConverters;
using AmSpaceTools.Views;
using AutoMapper;
using ExcelWorker;
using ExcelWorker.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class IdpTranslationsPreviewViewModel : BaseViewModel, IProgressReporter
    {
        private IEnumerable<IdpColumn> _excelColumnsPreview;
        private IExcelWorker _excelWorker;
        private ICommand _openFileCommand;
        private IEnumerable<IdpExcelRow> _allRows;
        private ICommand _uploadDataCommand;
        private string _currentFilePath;
        private IMapper _mapper;
        private IAmSpaceClient _client;
        private ObservableCollection<ColumnDefinitionError> _errors;
        private int _similarityPercent;
        public ProgressIndicatorViewModel ProgressVM { get; private set; }


        public ObservableCollection<ColumnDefinitionError> Errors { get => _errors; set => _errors = value; }

        public bool IsUploadVisible
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentFilePath)) return false;
                Errors.Clear();
                if (!ExcelColumnsPreview.Any(_ => _.ColumnType == ColumnActionType.SourceText))
                {
                    Errors.Add(new ColumnDefinitionError(ColumnActionType.SourceText));
                    return false;
                }
                if (!ExcelColumnsPreview.Any(_ => _.ColumnType == ColumnActionType.Translation))
                {
                    Errors.Add(new ColumnDefinitionError(ColumnActionType.Translation));
                    return false;
                }
                else
                {
                    foreach(var column in ExcelColumnsPreview.Where(_ => _.ColumnType == ColumnActionType.Translation))
                    {
                        if (string.IsNullOrEmpty(column.Language))
                        {
                            Errors.Add(new ColumnDefinitionError(ColumnActionType.Translation));
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public int SimilarityPercent
        {
            get { return _similarityPercent; }
            set { _similarityPercent = value; }
        }

        public IEnumerable<IdpColumn> ExcelColumnsPreview
        {
            get
            {
                if (_excelColumnsPreview == null) _excelColumnsPreview = new List<IdpColumn>();
                return _excelColumnsPreview;
            }
            set
            {
                _excelColumnsPreview = value;
                OnPropertyChanged();
            }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set
            {
                _currentFilePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUploadVisible));
                OnPropertyChanged(nameof(PreviewIsNotLoaded));
            }
        }

        public bool PreviewIsNotLoaded
        {
            get
            {
                return string.IsNullOrEmpty(CurrentFilePath);
            }
        }

        public IdpTranslationsPreviewViewModel(IExcelWorker excelWorker, IMapper mapper, IAmSpaceClient client, ProgressIndicatorViewModel vm)
        {
            _excelWorker = excelWorker;
            _mapper = mapper;
            _client = client;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
            Errors = new ObservableCollection<ColumnDefinitionError>();
            _similarityPercent = 100;
            ProgressVM = vm;
        }

        private async void UploadData(object obj)
        {
            ProgressVM.ShowLoading();
            ProgressVM.ReportProgress(new ProgressState { ProgressTasksDone = 0, ProgressDescriptionText = "Collecting information..." });
            var competencies = await _client.GetCompetenciesAsync();
            var allAmSpaceActions = new Dictionary<Competency, List<IdpAction>>();
            _allRows =  await _excelWorker.GetAllRowsAsync(ExcelColumnsPreview);
            var uniqueActions = _allRows.NormalizeTranslations();
            int i = 0;
            foreach (var competency in competencies)
            {
                ProgressVM.ReportProgress(new ProgressState { ProgressTasksDone = ++i, ProgressTasksTotal = competencies.Count(), ProgressDescriptionText = $"{competency.Name} lvl {competency.Level.Name} processing" });
                if (ProgressVM.IsProgressCancelled) break;
                if (competency.ActionCount == 0) continue;
                var compActions = await _client.GetCompetencyActionsAsync(competency.Id.Value);
                allAmSpaceActions.UpsertKey(competency).AddRange(compActions.Actions);
                foreach (var action in compActions.Actions)
                {
                    var translationKey = await uniqueActions.FindSimilarAsync(action.Name, SimilarityPercent);
                    if (translationKey.Value == null) continue;
                    foreach (var translation in translationKey.Value)
                        action.Translations.UpsertTranslation(translation);
                    action.Updated = true;
                    _allRows.Where(_ => _.ActionSourceDescription == translationKey.Key).ForEach(_ => _.Taken = true);
                }
                var transformedActions = _mapper.Map<UpdateAction>(compActions);
                var result = await _client.UpdateActionAsync(transformedActions, competency.Id.Value);
            }
            DetermineMissingMatchingActions(allAmSpaceActions);
            ProgressVM.CloseLoading();
        }

        private void DetermineMissingMatchingActions(IDictionary<Competency, List<IdpAction>> compActions)
        {
            var targetActions = _mapper.Map<IEnumerable<IdpExcelRow>>(compActions);
            SaveUploadResults(
                _allRows.Where(_ => !_.Taken),
                targetActions.Where(_ => _.Taken));
        }

        protected async void SaveUploadResults(IEnumerable<IdpExcelRow> missingActions, IEnumerable<IdpExcelRow> matchingActions)
        {
            var fileName =
                $"{DateTime.Now.Year}_" +
                $"{DateTime.Now.Month}_" +
                $"{DateTime.Now.Day}_" +
                $"{DateTime.Now.Hour}-" +
                $"{DateTime.Now.Minute}-" +
                $"{DateTime.Now.Second}" +
                $"_Missing.xlsx";
            await _excelWorker.SaveDataAsync(fileName, AmSpaceModels.Enums.AppDataFolders.Reports, missingActions, "Missing");

            fileName = 
                $"{DateTime.Now.Year}_" +
                $"{DateTime.Now.Month}_" +
                $"{DateTime.Now.Day}_" +
                $"{DateTime.Now.Hour}-" +
                $"{DateTime.Now.Minute}-" +
                $"{DateTime.Now.Second}" +
                $"_Matching.xlsx";
            await _excelWorker.SaveDataAsync(fileName, AmSpaceModels.Enums.AppDataFolders.Reports, matchingActions, "Matching");
        }

        public ICommand UploadDataCommand
        {
            get => _uploadDataCommand; 
            set
            {
                _uploadDataCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenFileCommand
        {
            get { return _openFileCommand; }
            set { _openFileCommand = value; }
        }

        private async void OpenFile(object obj)
        {
            IsLoading = true;
            var dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                CurrentFilePath = dialog.FileName;
                await _excelWorker.OpenFileAsync(CurrentFilePath);
                ExcelColumnsPreview = _excelWorker.GetColumnDataPreview(10);
                foreach(var excelColumn in ExcelColumnsPreview)
                    excelColumn.PropertyChanged += ExcelColumn_PropertyChanged;
            }
            IsLoading = false;
        }

        private void ExcelColumn_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsUploadVisible));
        }
    }
}
