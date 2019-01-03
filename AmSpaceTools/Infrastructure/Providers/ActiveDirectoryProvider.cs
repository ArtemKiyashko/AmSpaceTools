using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure.Providers
{
    public class ActiveDirectoryProvider : IActiveDirectoryProvider
    {
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

        private PrincipalContext _principalContext;

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
    }
}
