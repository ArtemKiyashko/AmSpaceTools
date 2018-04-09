using AmSpaceClient;
using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.ModelConverters;
using AutoMapper;
using ExcelWorker;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmSpaceTools.ViewModels
{
    public class IdpTranslationsPreviewViewModel : BaseViewModel
    {
        private IEnumerable<IdpExcelColumn> _excelColumnsPreview;
        private IExcelWorker<CompetencyActionDto> _excelWorker;
        private ICommand _openFileCommand;
        private IEnumerable<IdpExcelRow> _allRows;
        private ICommand _uploadDataCommand;
        private string _currentFilePath;
        private IMapper _mapper;
        private IAmSpaceClient _client;

        public bool IsUploadVisible
        {
            get
            {
                return !string.IsNullOrEmpty(CurrentFilePath);
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
            }
        }

        public IdpTranslationsPreviewViewModel(IExcelWorker<CompetencyActionDto> excelWorker, IMapper mapper, IAmSpaceClient client)
        {
            _excelWorker = excelWorker;
            _mapper = mapper;
            _client = client;
            OpenFileCommand = new RelayCommand(OpenFile);
            UploadDataCommand = new RelayCommand(UploadData);
        }

        private async void UploadData(object obj)
        {
            IsLoading = true;
            var competencies = await _client.GetCompetenciesAsync();
            var levels = await _client.GetLevelsAsync();
            foreach (var competency in competencies)
            {
                var compActions = await _client.GetCompetencyActionsAsync(competency.Id);
                foreach(var action in compActions.Actions)
                {
                    var translationKey = AllRows.FirstOrDefault(_ => _.ActionSourceDescription == action.Name);
                    if (translationKey == null) continue;
                    foreach(var translation in translationKey.Translations)
                    {
                        var oldTranslation = action.Translations.FirstOrDefault(_ => _.Language == translation.Language);
                        if (oldTranslation == null)
                        {
                            action.Translations.Add(new Translation
                            {
                                Language = translation.Language,
                                Name = translation.Name
                            });
                        }
                        else
                        {
                            oldTranslation.Name = translation.Name;
                        }
                    }
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
                ExcelColumnsPreview = _excelWorker.GetColumnDataPreview(CurrentFilePath, 6);
            }
            IsLoading = false;
        }
    }
}
