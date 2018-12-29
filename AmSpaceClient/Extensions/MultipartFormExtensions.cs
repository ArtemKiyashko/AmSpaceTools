using AmSpaceClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.AmSpaceClient
{
    public static class MultipartFormExtensions
    {
        public static void FillForm(this MultipartFormDataContent form, IEnumerable<KeyValuePair<string, string>> parameters, IEnumerable<FileToUpload> files)
        {
            foreach (var parameter in parameters)
                form.Add(new StringContent(parameter.Value), parameter.Key);
            foreach (var file in files)
                form.Add(new StreamContent(new MemoryStream(file.Data)), file.DataName, file.FileName);
        }
    }
}
