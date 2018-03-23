using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public class CsvLogAppender : RollingFileAppender
    {
        protected override void WriteHeader()
        {
            if (LockingModel.AcquireLock().Length == 0)
            {
                base.WriteHeader();
            }
        }
    }
}
