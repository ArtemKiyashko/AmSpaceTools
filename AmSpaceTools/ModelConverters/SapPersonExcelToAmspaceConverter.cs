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
            externalUser.Sex = (AmSpaceModels.Enums.Sex)source.Sex;
            externalUser.StartDate = source.ContractStartDate;
            externalUser.EndDate = source.ContractEndDate;
            externalUser.Level = source.Level;
            externalUser.Status = (AmSpaceModels.Enums.UserStatus)source.Status;
            externalUser.DomainId = source.Mpk;
            externalUser.CountryCode = source.Country;
            externalUser.ManagerLegalId = source.ManagerId;
            externalUser.ContractNumber = source.ContractNumber;
            return externalUser;
        }
    }
}
