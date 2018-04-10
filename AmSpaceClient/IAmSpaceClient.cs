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
        Task<bool> LoginRequestAsync(string userName, SecureString password);
        Task<Profile> ProfileRequestAsync();
        Task<IEnumerable<Competency>> GetCompetenciesAsync();
        Task<IEnumerable<Level>> GetLevelsAsync();
        Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId);
        Task UpdateActionAsync(UpdateAction model, long competencyId);
        Task LogoutRequestAsync();
        Task<BitmapSource> GetAvatarAsync(string link);
    }
}
