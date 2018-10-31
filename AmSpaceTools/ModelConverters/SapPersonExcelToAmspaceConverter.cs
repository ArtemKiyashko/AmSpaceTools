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
            externalUser.Sex = (AmSpaceModels.Enums.SapSex)source.Sex;
            externalUser.StartDate = source.ContractStartDate;
            externalUser.EndDate = source.ContractEndDate;
            externalUser.Level = source.Level;
            externalUser.Mpk = source.Mpk;
            externalUser.CountryCode = source.Country;
            externalUser.ContractNumber = source.ContractNumber;
            switch (source.Status)
            {
                case ContractStatus.Active:
                    externalUser.Status = AmSpaceModels.Enums.SapUserStatus.ACTIVE;
                    break;
                case ContractStatus.Terminated:
                    externalUser.Status = AmSpaceModels.Enums.SapUserStatus.TERMINATED;
                    break;
                case ContractStatus.Suspended:
                    externalUser.Status = AmSpaceModels.Enums.SapUserStatus.SUSPENDED;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(source.Status), $"Unrecognized {nameof(source.Status)} {source.Status}");
            }
            return externalUser;
        }
    }
}
