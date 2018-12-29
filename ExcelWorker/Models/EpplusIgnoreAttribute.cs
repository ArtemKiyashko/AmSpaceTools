using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EpplusIgnoreAttribute : Attribute
    {
    }
}
