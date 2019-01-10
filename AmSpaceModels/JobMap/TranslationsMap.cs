using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.JobMap
{
    public static class TranslationsMap
    {
        public static Dictionary<string, string> Map { get; } = new Dictionary<string, string>
        {
            ["EN"] = "en",
            ["PL"] = "pl",
            ["CZ"] = "cs",
            ["HU"] = "hu",
            ["SK"] = "sk",
            ["BG"] = "bg",
            ["DE"] = "de",
            ["ES"] = "es",
            ["RO"] = "ro",
            ["RU"] = "ru",
            ["RS"] = "rs",
            ["ZH-HANS"] = "zh-hans",
            ["CN"] = "zh-hans",
            ["FR"] = "fr",
            ["HR"] = "hr"
        };
}
}
