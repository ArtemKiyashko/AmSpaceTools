using AmSpaceModels;
using EPPlus.Core.Extensions;
using ExcelWorker.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker
{
    public static class EpplusExtensions
    {
        public static ExcelRangeBase LoadFromCollectionFiltered<T>(this ExcelRangeBase @this, IEnumerable<T> collection, bool printHeaders) where T : class
        {
            MemberInfo[] membersToInclude = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnoreAttribute)))
                .ToArray();

            return @this.LoadFromCollection<T>(collection, printHeaders,
                OfficeOpenXml.Table.TableStyles.None,
                BindingFlags.Instance | BindingFlags.Public,
                membersToInclude);
        }

        public static bool AttributesExists<T>(this IEnumerable<T> data)
        {
            return typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Any(p => Attribute.IsDefined(p, typeof(ExcelTableColumnAttribute)));
        }
    }
}
