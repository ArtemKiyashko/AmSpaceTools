using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public interface IProgressState
    {
        int ProgressPercent { get ; }
        int ProgressTasksDone { get; set; }
        int ProgressTasksTotal { get; set; }

    }

    public struct ProgressState : IProgressState
    {
        public int ProgressPercent => Convert.ToInt32(ProgressTasksTotal != 0 ? (double)ProgressTasksDone / (double)ProgressTasksTotal * 100 : 100);
        public int ProgressTasksDone { get ; set ; }
        public int ProgressTasksTotal { get ; set ; }
    }
}
