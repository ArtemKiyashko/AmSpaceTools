using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AmSpaceClient
{
    public interface IAmSpaceClient
    {
        Task<bool> LoginRequestAsync(string userName, SecureString password, IAmSpaceEnvironment environment);
        Task<Profile> ProfileRequestAsync(int? profileId = null);
        Task<IEnumerable<Competency>> GetCompetenciesAsync();
        Task<IEnumerable<Level>> GetLevelsAsync();
        Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId);
        Task<bool> UpdateActionAsync(UpdateAction model, long competencyId);
        Task<bool> LogoutRequestAsync();
        Task<BitmapSource> GetAvatarAsync(string link);
        Task<AmspaceDomain> GetOrganizationStructureAsync(int rootDomainId);
        Task<IEnumerable<AmspaceUser>> GetDomainUsersAsync(int domainId);
        Task<bool> PutUserAsync(SapUser user);
        Task<bool> PutDomainAsync(SapDomain domain);
        Task<bool> DisableUserAsync(SapUser user);
        Task<bool> DisableDomainAsync(SapDomain domain);
    }
}
