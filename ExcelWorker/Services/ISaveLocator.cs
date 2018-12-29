using AmSpaceModels.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Services
{
    public interface ISaveLocator
    {
        string GetSaveLocation(string filename, AppDataFolders folder);
    }

    public class SaveLocator : ISaveLocator
    {
        public string GetSaveLocation(string filename, AppDataFolders folder)
        {
            var appDirectory = FoldersLocations.GetFolderLocation(folder);
            if (!Directory.Exists(appDirectory))
                Directory.CreateDirectory(appDirectory);
            return Path.Combine(appDirectory, Path.GetFileName(filename));
        }
    }
}
