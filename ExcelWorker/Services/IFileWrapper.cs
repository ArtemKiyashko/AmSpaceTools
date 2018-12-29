using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Services
{
    public interface IFileWrapper
    {
        Stream GetStream(string path, FileMode mode, FileAccess access, FileShare share);
    }

    public class FileWrapper : IFileWrapper
    {
        public Stream GetStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(path, mode, access, share);
        }
    }
}
