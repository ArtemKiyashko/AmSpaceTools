using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Enums
{
    public enum Languages
    {
        English,
        Polish,
        Czech,
        Hungarian,
        Slovakian,
        Bulgarian,
        German,
        Espanian,
        Romanian,
        Russian,
        Serbian,
        Chinees,
        French,
        Croatian
    }

    public static class LanguageMap
    {
        public static Dictionary<Languages, string> LanguageCode => new Dictionary<Languages, string>
        {
            { Languages.English, "en"},
            { Languages.Polish, "pl"},
            { Languages.Czech, "cs"},
            { Languages.Hungarian, "hu"},
            { Languages.Slovakian, "sk"},
            { Languages.Bulgarian, "bg"},
            { Languages.German, "de"},
            { Languages.Espanian, "es"},
            { Languages.Romanian, "ro"},
            { Languages.Russian, "ru"},
            { Languages.Serbian, "rs"},
            { Languages.Chinees, "zh-hans"},
            { Languages.French, "fr"},
            { Languages.Croatian, "hr"}
        };
    }
}
