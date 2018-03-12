using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public interface IAmSpaceClient
    {
        Task<bool> LoginRequestAsync(string userName, SecureString password);
        Task<Profile> ProfileRequestAsync();
        Task<IEnumerable<Competency>> GetCompetenciesAsync(List<Competency> competencies, string pageUrl);
        Task<IEnumerable<Level>> GetLevelsAsync();
        Task<IEnumerable<CompetencyAction>> GetCompetencyActionsAsync();
        Task UpdateActionAsync(UpdateAction model, long competencyId);
    }
}
