using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Idp
{
    public class IdpExcelColumn : BaseModel
    {
        private ColumnActionType _columnType;

        public string ColumnAddress { get; set; }
        public ColumnActionType ColumnType
        {
            get => _columnType;
            set
            {
                _columnType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLanguageVisible));
            }
        }
        public IEnumerable<string> ColumnData { get; set; }
        public int ColumnIndex { get; set; }
        public int WorkSheet { get; set; }
        public string Language
        { get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        public bool IsLanguageVisible
        {
            get
            {
                return ColumnType == ColumnActionType.Translation;
            }
        }

        private IEnumerable<string> _langs;
        private string _language;

        public IEnumerable<string> Langs
        {
            get
            {
                if (_langs == null)
                    _langs = new List<string>
                    {
                        "en",
                        "pl",
                        "cs",
                        "hu",
                        "sk",
                        "bg",
                        "de",
                        "es",
                        "ro",
                        "ru",
                        "rs",
                        "zh-hans",
                        "fr",
                        "hr"
                    };
                return _langs;
            }
        }
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
