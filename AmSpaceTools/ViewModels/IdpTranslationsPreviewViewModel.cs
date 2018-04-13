using AmSpaceClient;
using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.ModelConverters;
using AutoMapper;
using ExcelWorker;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class IdpTranslationsPreviewViewModel : BaseViewModel
    {
        private IEnumerable<IdpExcelColumn> _excelColumnsPreview;
        private IExcelWorker _excelWorker;
        private ICommand _openFileCommand;
        private IEnumerable<IdpExcelRow> _allRows;
        private ICommand _uploadDataCommand;
        private string _currentFilePath;
        private IMapper _mapper;
        private IAmSpaceClient _client;
        private ObservableCollection<ColumnDefinitionError> _errors;

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


        public IEnumerable<IdpExcelColumn> ExcelColumnsPreview
        {
            get
            {
                if (_excelColumnsPreview == null) _excelColumnsPreview = new List<IdpExcelColumn>();
                return _excelColumnsPreview;
            }
            set
            {
                _excelColumnsPreview = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<IdpExcelRow> AllRows
        {
            get
            {
                if (_allRows == null)
                    _allRows = _excelWorker.GetAllRows(CurrentFilePath, ExcelColumnsPreview);
                return _allRows;
            }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set
            {
                _currentFilePath = value;
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

        public IdpTranslationsPreviewViewModel(IExcelWorker excelWorker, IMapper mapper, IAmSpaceClient client)
        {
            _excelWorker = excelWorker;
            _mapper = mapper;
            _client = client;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
            Errors = new ObservableCollection<ColumnDefinitionError>();
        }

        private void UpsertTranslation(Translation newTranslation, IList<Translation> currentTranslations)
        {
            var oldTranslation = currentTranslations.FirstOrDefault(_ => _.Language == newTranslation.Language);
            if (oldTranslation == null)
            {
                currentTranslations.Add(new Translation
                {
                    Language = newTranslation.Language,
                    Name = newTranslation.Name
                });
            }
            else
            {
                oldTranslation.Name = newTranslation.Name;
            }
        }

        private async void UploadData(object obj)
        {
            IsLoading = true;
            var competencies = await _client.GetCompetenciesAsync();
            var levels = await _client.GetLevelsAsync();
            foreach (var competency in competencies)
            {
                var compActions = await _client.GetCompetencyActionsAsync(competency.Id);
                foreach (var action in compActions.Actions)
                {
                    var translationKey = AllRows.FirstOrDefault(_ => _.ActionSourceDescription == action.Name);
                    if (translationKey == null) continue;
                    foreach (var translation in translationKey.Translations)
                        UpsertTranslation(translation, action.Translations);
                }
                var transformedActions = _mapper.Map<UpdateAction>(compActions);
                await _client.UpdateActionAsync(transformedActions, competency.Id);
            }
            IsLoading = false;
        }

        public ICommand UploadDataCommand
        {
            get { return _uploadDataCommand; }
            set { _uploadDataCommand = value; }
        }

        public ICommand OpenFileCommand
        {
            get { return _openFileCommand; }
            set { _openFileCommand = value; }
        }

        private void OpenFile(object obj)
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
                ExcelColumnsPreview = _excelWorker.GetColumnDataPreview(CurrentFilePath, 10);
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
