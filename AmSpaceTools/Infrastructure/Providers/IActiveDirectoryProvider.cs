using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure.Providers
{
    public interface IActiveDirectoryProvider : IDisposable
    {
        /// <summary>
        /// Find all AD users by email wildcard
        /// </summary>
        /// <param name="email"></param>
        /// <returns>UserPrincipal collection found by filter or empty collection if nothing found</returns>
        IEnumerable<Principal> FindAllByEmail(string email);
        /// <summary>
        /// Find one AD user by email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns>UserPrincipal found by filter or null if nothing found</returns>
        Principal FindOneByEmail(string email);
        /// <summary>
        /// Return true if connected to LDAP server, otherwise false
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// Firing when connection status changed
        /// </summary>
        event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;
    }
}
