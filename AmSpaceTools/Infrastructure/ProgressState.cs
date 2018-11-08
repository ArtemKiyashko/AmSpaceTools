using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public interface IProgressState
    {
        Status ProgressStatus { get; set; }
        int ProgressPercent { get ; set; }
        int ProgressTasksDone { get; set; }
        int ProgressTasksTotal { get; set; }

    }

    public enum Status
    {
        Preparations,
        Uploading,
        Finishing
    }
    public struct ProgressState : IProgressState
    {
        private int _progressPrecent;
        public Status ProgressStatus { get; set; }
        public int ProgressPercent { get => ProgressTasksDone / (ProgressTasksTotal == 0 ? ProgressTasksDone : ProgressTasksTotal) * 100; set => _progressPrecent = value ; }
        public int ProgressTasksDone { get ; set ; }
        public int ProgressTasksTotal { get ; set ; }
    }
}
