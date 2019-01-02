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
        /// <returns></returns>
        IEnumerable<Principal> FindAllByEmail(string email);
        /// <summary>
        /// Find one AD user by email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Principal FindOneByEmail(string email);
    }
}
