using AmSpaceModels.Enums;
using AmSpaceModels.Organization;
using AmSpaceTools.Infrastructure.Providers;
using AutoMapper;
using ExcelWorker.Models;


namespace AmSpaceTools.ModelConverters
{
    public class SapPersonExcelToAmspaceConverter : ITypeConverter<SapPersonExcelRow, ExternalAccount>
    {
        private readonly IActiveDirectoryProvider _activeDirectoryProvider;

        public SapPersonExcelToAmspaceConverter(IActiveDirectoryProvider activeDirectoryProvider)
        {
            _activeDirectoryProvider = activeDirectoryProvider;
        }

        public ExternalAccount Convert(SapPersonExcelRow source, ExternalAccount destination, ResolutionContext context)
        {

            var externalUser = new ExternalAccount();
            externalUser.FirstName = source.Name;
            externalUser.LastName = source.Surname;
            externalUser.Email = source.Email;
            externalUser.PhoneNumber = source.Phone;
            externalUser.PersonLegalId = source.IdentityNumber;
            externalUser.DateOfBirth = source.BirthDate;
            externalUser.Nationality = source.Nationality;
            externalUser.Sex = (AmSpaceSex)source.Sex;
            externalUser.StartDate = source.ContractStartDate;
            externalUser.EndDate = source.ContractEndDate;
            externalUser.Level = source.Level;
            externalUser.Mpk = source.Mpk;
            externalUser.CountryCode = source.Country;
            externalUser.ContractNumber = source.ContractNumber;
            externalUser.Status = (AmSpaceUserStatus)source.Status;
            externalUser.BackendType = _activeDirectoryProvider.FindOneByEmail(source.Email) != null
               ? AccountBackendType.ActiveDirectory
               : AccountBackendType.AmSpace;
            return externalUser;
        }
    }
}
