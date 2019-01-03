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
        public bool IsConnected { get; private set; }

        private PrincipalContext _principalContext;

        public ActiveDirectoryProvider()
        {
            try
            {
                _principalContext = new PrincipalContext(ContextType.Domain);
                IsConnected = true;
            }
            catch (Exception)
            {
                _principalContext = null;
                IsConnected = false;
            }
        }

        public void Dispose()
        {
            _principalContext.Dispose();
            _principalContext = null;
            IsConnected = false;
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
