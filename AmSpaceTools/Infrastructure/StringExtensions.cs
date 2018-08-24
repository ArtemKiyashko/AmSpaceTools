using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public static class StringExtensions
    {
        public static string ToSha1(this string input)
        {
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }
    }
}
