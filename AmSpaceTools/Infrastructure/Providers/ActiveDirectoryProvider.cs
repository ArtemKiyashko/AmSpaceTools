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
        private readonly PrincipalContext _principalContext;

        public ActiveDirectoryProvider()
        {
            _principalContext = new PrincipalContext(ContextType.Domain);
        }
        public void Dispose()
        {
            _principalContext.Dispose();
        }

        public IEnumerable<Principal> FindAllByEmail(string email)
        {
            using (var searcher = new PrincipalSearcher(new UserPrincipal(_principalContext) { EmailAddress = email }))
            {
                return searcher.FindAll().ToList();
            }
        }

        public Principal FindOneByEmail(string email)
        {
            using (var searcher = new PrincipalSearcher(new UserPrincipal(_principalContext) { EmailAddress = email }))
            {
                return searcher.FindOne();
            }
        }
    }
}
