using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels;

namespace AmSpaceClient
{
    public class FakeClient : IAmSpaceClient
    {
        public Task<IEnumerable<Competency>> GetCompetenciesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CompetencyAction>> GetCompetencyActionsAsync(long competencyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Level>> GetLevelsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoginRequestAsync(string userName, SecureString password) => Task.FromResult(true);

        public Task<Profile> ProfileRequestAsync() => Task.FromResult(new Profile());

        public Task UpdateActionAsync(UpdateAction model, long competencyId)
        {
            throw new NotImplementedException();
        }
    }
}
