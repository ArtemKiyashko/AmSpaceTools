using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure.Providers
{
    public class ConnectionStatusEventArgs : EventArgs
    {
        public bool IsConnected { get; }
        public ConnectionStatusEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}
