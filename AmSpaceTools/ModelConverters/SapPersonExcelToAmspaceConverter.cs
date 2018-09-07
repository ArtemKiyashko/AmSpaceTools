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
    public class SapPersonExcelToAmspaceConverter : ITypeConverter<IEnumerable<SapPersonExcelRow>, IEnumerable<SapUser>>
    {
        public IEnumerable<SapUser> Convert(IEnumerable<SapPersonExcelRow> source, IEnumerable<SapUser> destination, ResolutionContext context)
        {
            var result = new List<SapUser>();
            foreach(var key in source.GroupBy(_ => _.IdentityNumber))
            {
                foreach (var contract in key)
                {
                    var sapUser = new SapUser();
                    sapUser.LdapName = string.Empty;
                    sapUser.Hash = contract.IdentityNumber.ToSha1();
                    sapUser.FirstName = contract.Name;
                    sapUser.LastName = contract.Surname;
                    sapUser.Email = contract.Email;
                    sapUser.PhoneNumber = contract.Phone;
                    sapUser.PersonLegalId = contract.IdentityNumber;
                    sapUser.DateOfBirth = contract.BirthDate;
                    sapUser.Nationality = contract.Nationality;
                    sapUser.Sex = (AmSpaceModels.Sap.Sex)contract.Sex;
                    //sapUser.MainEmployeeId = key.First(_ => _.ContractNumber == 1).EmployeeId;
                    //sapUser.EmployeeId = contract.EmployeeId;
                    //sapUser.ManagerEmployeeId = contract.ManagerId;
                    sapUser.StartDate = contract.ContractStartDate;
                    sapUser.EndDate = contract.ContractEndDate;
                    sapUser.Level = contract.Level;
                    sapUser.Status = (int)contract.Status;
                    sapUser.DomainId = contract.Mpk;
                    sapUser.CountryCode = contract.Country;
                    result.Add(sapUser);
                }
            }
            return result;
        }
    }
}
