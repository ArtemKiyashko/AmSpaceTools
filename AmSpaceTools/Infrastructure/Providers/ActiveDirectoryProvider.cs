using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AmSpaceTools.Infrastructure.Providers
{
    public class ActiveDirectoryProvider : IActiveDirectoryProvider
    {
        private PrincipalContext _principalContext;
        private readonly DispatcherTimer _activeDirectoryStatusTimer;
        private bool _connectionCurrentStatus;

        public bool IsConnected
        {
            get
            {
                if (_principalContext == null || string.IsNullOrEmpty(_principalContext.ConnectedServer))
                    return false;
                try
                {
                    new PrincipalContext(ContextType.Domain);
                }
                catch (PrincipalServerDownException)
                {
                    return false;
                }
                return true;
            }
        }

        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public ActiveDirectoryProvider()
        {
            try
            {
                _principalContext = new PrincipalContext(ContextType.Domain);
            }
            catch (Exception)
            {
                _principalContext = null;
            }
            _connectionCurrentStatus = false;
            _activeDirectoryStatusTimer = new DispatcherTimer();
            _activeDirectoryStatusTimer.Interval = TimeSpan.FromSeconds(2);
            _activeDirectoryStatusTimer.Tick += _activeDirectoryStatusTimer_Tick;
            _activeDirectoryStatusTimer.Start();
        }

        public void Dispose()
        {
            _principalContext.Dispose();
            _principalContext = null;
        }

        public IEnumerable<Principal> FindAllByEmail(string email)
        {
            if (!IsConnected) return Enumerable.Empty<Principal>();
            using (var searcher = new PrincipalSearcher(new UserPrincipal(_principalContext) { EmailAddress = email }))
            {
                return searcher.FindAll().ToList();
            }
        }

        public Principal FindOneByEmail(string email)
        {
            if (!IsConnected) return null;
            using (var searcher = new PrincipalSearcher(new UserPrincipal(_principalContext) { EmailAddress = email }))
            {
                return searcher.FindOne();
            }
        }

        private void OnConnectionStatusChanged(object sender, ConnectionStatusEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(sender, e);
        }

        private void _activeDirectoryStatusTimer_Tick(object sender, EventArgs e)
        {
            var status = IsConnected;
            if (_connectionCurrentStatus != status)
            {
                _connectionCurrentStatus = status;
                OnConnectionStatusChanged(sender, new ConnectionStatusEventArgs(_connectionCurrentStatus));
            }
        }
    }
}
