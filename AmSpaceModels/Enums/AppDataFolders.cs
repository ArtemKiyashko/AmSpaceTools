using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Enums
{
    public enum AppDataFolders
    {
        Reports,
        Logs,
        Common
    }

    public static class FoldersLocations
    {
        private static readonly Dictionary<AppDataFolders, string> _locations;
        static FoldersLocations()
        {
            _locations = new Dictionary<AppDataFolders, string>
            {
                { AppDataFolders.Common, "" },
                { AppDataFolders.Logs, "Logs"},
                { AppDataFolders.Reports, "Reports"}
            };
        }

        public static string GetFolderLocation(AppDataFolders dataFolder) => 
           Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName), _locations[dataFolder]);
    }
}
