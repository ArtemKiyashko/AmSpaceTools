using AmSpaceModels.Organization;
using ExcelWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceToolsTests.Infrastructure
{
    public class SapPersonExcelRowComparable : SapPersonExcelRow, IComparable
    {
        public int CompareTo(object obj)
        {
            var res = obj as ExternalAccount;
            if (res == null) return -1;

            if (!res.FirstName.Equals(Name)
                || !res.LastName.Equals(Surname)
                || (int)res.Status != (int)Status
                || (int)res.Sex != (int)Sex
                || !res.PhoneNumber.Equals(Phone)
                || !res.Nationality.Equals(Nationality)
                || !res.CountryCode.Equals(Country)
                || !res.Mpk.Equals(Mpk)
                || !res.Level.Equals(Level)
                || !res.PersonLegalId.Equals(IdentityNumber)
                || !res.StartDate.Equals(ContractStartDate)
                || !res.EndDate.Equals(ContractEndDate)
                || !res.ContractNumber.Equals(ContractNumber)
                || !res.DateOfBirth.Equals(BirthDate)
                || !res.Email.Equals(Email))
                return -1;
            return 0;
        }
    }
}
