using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public class FileToUpload
    {
        public byte[] Data { get; set; }
        public string DataName { get; set; }
        public string FileName { get; set; }
    }
}
