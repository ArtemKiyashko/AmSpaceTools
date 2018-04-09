using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels;

namespace AmSpaceClient
{
    public class AmSpaceClient : IAmSpaceClient
    {
        public Task<IEnumerable<Competency>> GetCompetenciesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Level>> GetLevelsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoginRequestAsync(string userName, SecureString password)
        {
            throw new NotImplementedException();
        }

        public Task<Profile> ProfileRequestAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateActionAsync(UpdateAction model, long competencyId)
        {
            throw new NotImplementedException();
        }
    }
}
