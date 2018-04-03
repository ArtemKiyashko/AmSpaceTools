using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public class IdpExcelColumn : BaseModel
    {
        private ColumnActionType _columnType;

        public string ColumnAddress { get; set; }
        public ColumnActionType ColumnType { get => _columnType;
            set
            {
                _columnType = value;
                OnPropertyChanged(nameof(IsLanguageVisible));
            }
        }
        public IEnumerable<string> ColumnData { get; set; }
        public int ColumnIndex { get; set; }
        public int WorkSheet { get; set; }
        public TranslationLanguage Language { get; set; }

        public bool IsLanguageVisible
        {
            get
            {
                return ColumnType == ColumnActionType.Translation;
            }
        }
    }

    public enum TranslationLanguage
    {
        PL,
        EN,
        BG,
        SK
    }

    public enum ColumnActionType
    {
        NotSpecified = 0,
        Translation = 1,
        SourceText = 2,
        CompetencyName = 3,
        CompetencyLevel = 4,
        ActionPercentage = 5
    }
}
