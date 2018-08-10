using AmSpaceModels;
using AmSpaceModels.Idp;
using AmSpaceModels.Organization;
using AmSpaceModels.Performance;
using AmSpaceModels.Sap;
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
        Task<Profile> GetProfileAsync();
        Task<Profile> GetProfileByIdAsync(long Id);
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
        Task<IEnumerable<Position>> GetPositionsInLevelsAsync(IEnumerable<Level> levels);
        Task<IEnumerable<Position>> GetPositionsAsync();
        Task<IEnumerable<People>> GetPeopleAsync();
        Task<IEnumerable<People>> GetPeopleInPositionsAsync(IEnumerable<Position> positions);
        Task<IEnumerable<Kpi>> GetFinancialKpiAsync(ContractDatum userContract);
        Task<IEnumerable<Kpi>> GetNonFinancialKpiAsync(ContractDatum userContract);
        Task<IEnumerable<Goal>> GetGoalsAsync(ContractDatum userContract, Roadmap roadmap);
        Task<Roadmaps> GetRoadmapsAsync(ContractDatum userContract);
        Task<Kpi> CreateFinancialKpiAsync(ContractDatum userContract, Kpi kpi);
        Task<Kpi> CreateNonFinancialKpiAsync(ContractDatum userContract, Kpi kpi);
        Task<Kpi> UpdateFinancialKpiAsync(ContractDatum userContract, Kpi kpi);
        Task<Kpi> UpdateNonFinancialKpiAsync(ContractDatum userContract, Kpi kpi);
        Task<Roadmap> CreateRoadmapAsync(ContractDatum userContract, Roadmap roadmap);
        Task<Goal> CreateGoalAsync(ContractDatum userContract, Roadmap roadmap, Goal goal);
        Task<Goal> UpdateGoalAsync(ContractDatum userContract, Roadmap roadmap, Goal goal);
        Task<bool> DeleteGoalAsync(ContractDatum userContract, Roadmap roadmap, Goal goal);
        Task<bool> DeleteFinancialKpiAsync(ContractDatum userContract, Kpi kpi);
        Task<bool> DeleteNonFinancialKpiAsync(ContractDatum userContract, Kpi kpi);
    }
}
