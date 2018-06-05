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
        Task<IEnumerable<AmspaceDomain>> GetOrganizationStructureAsync();
        Task<IEnumerable<AmspaceUser>> GetDomainUsersAsync(int domainId);
        /// <summary>
        /// Allows only Create and Update users
        /// </summary>
        /// <param name="user"></param>
        /// <returns>True in case of successful request</returns>
        Task<bool> PutUserAsync(SapUser user);
        /// <summary>
        /// Enables all changes to Domains: Create, Update and Disable Domains. To deactivate Domain,
        /// set "Status" prop false and "ParentDomainId" to null
        /// </summary>
        /// <param name="domain"></param>
        /// <returns>True in case of successful request</returns>
        Task<bool> PutDomainAsync(SapDomain domain);
        Task<bool> DisableUserAsync(SapUserDelete userToDelete);
    }
}
