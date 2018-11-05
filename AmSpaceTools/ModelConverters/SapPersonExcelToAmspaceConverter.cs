using AmSpaceModels.Organization;
using AmSpaceModels.Sap;
using AmSpaceTools.Infrastructure;
using AutoMapper;
using ExcelWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ModelConverters
{
    public class SapPersonExcelToAmspaceConverter : ITypeConverter<SapPersonExcelRow, ExternalAccount>
    {
        private Dictionary<ContractStatus, AmSpaceModels.Enums.SapUserStatus> _statusMapping = new Dictionary<ContractStatus, AmSpaceModels.Enums.SapUserStatus>
        {
            { ContractStatus.Active, AmSpaceModels.Enums.SapUserStatus.ACTIVE },
            { ContractStatus.Terminated, AmSpaceModels.Enums.SapUserStatus.TERMINATED },
            { ContractStatus.Suspended, AmSpaceModels.Enums.SapUserStatus.SUSPENDED }
        };
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
            externalUser.Sex = (AmSpaceModels.Enums.SapSex)source.Sex;
            externalUser.StartDate = source.ContractStartDate;
            externalUser.EndDate = source.ContractEndDate;
            externalUser.Level = source.Level;
            externalUser.Mpk = source.Mpk;
            externalUser.CountryCode = source.Country;
            externalUser.ContractNumber = source.ContractNumber;
            externalUser.Status = _statusMapping[source.Status];
            return externalUser;
        }
    }
}
