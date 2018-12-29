using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public interface IProgressState
    {
        decimal ProgressPercent { get ; }
        int ProgressTasksDone { get; set; }
        int ProgressTasksTotal { get; set; }
        string ProgressDescriptionText { get; set; }
    }

    public struct ProgressState : IProgressState
    {
        public decimal ProgressPercent => ProgressTasksTotal != 0 ? (decimal)ProgressTasksDone / (decimal)ProgressTasksTotal * 100 : 0;
        public int ProgressTasksDone { get ; set ; }
        public int ProgressTasksTotal { get ; set ; }
        public string ProgressDescriptionText { get ; set ; }
    }
}
