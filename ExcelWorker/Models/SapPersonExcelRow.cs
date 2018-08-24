using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPPlus.Core.Extensions;

namespace ExcelWorker.Models
{
    public class SapPersonExcelRow
    {
        [ExcelTableColumn("Validity_date ")]
        public DateTime ValidityDate { get; set; }

        [ExcelTableColumn("Export_date ")]
        public DateTime ExportDate { get; set; }

        [ExcelTableColumn("MPK")]
        public int Mpk { get; set; }

        [ExcelTableColumn("Emp_code  ")]
        public string EmployeeCode { get; set; }

        [ExcelTableColumn("Name")]
        public string Name { get; set; }

        [ExcelTableColumn("Surname ")]
        public string Surname { get; set; }

        [ExcelTableColumn("Emp_date ")]
        public DateTime EmployementDate { get; set; }

        [ExcelTableColumn("Position")]
        public string PositionCode { get; set; }

        [ExcelTableColumn("Position_Type")]
        public int Level { get; set; }

        [ExcelTableColumn("Phone ")]
        public string Phone { get; set; }

        [ExcelTableColumn("email")]
        public string Email { get; set; }

        [ExcelTableColumn("empl_id")]
        public int EmployeeId { get; set; }

        [ExcelTableColumn("sex")]
        public Sex Sex { get; set; }

        [ExcelTableColumn("date of birth ")]
        public DateTime BirthDate { get; set; }

        [ExcelTableColumn("identity number ")]
        public string IdentityNumber { get; set; }

        [ExcelTableColumn("nationality ")]
        public string Nationality { get; set; }

        [ExcelTableColumn("country_z  ")]
        public string Country { get; set; }

        [ExcelTableColumn("contract_number ")]
        public int ContractNumber { get; set; }

        [ExcelTableColumn("contract_valid_from ")]
        public DateTime ContractStartDate { get; set; }

        [ExcelTableColumn("contract_valid_to ")]
        public DateTime ContractEndDate { get; set; }

        [ExcelTableColumn("termination_date ")]
        public DateTime TerminationDate { get; set; }
    }
    public enum Sex
    {
        Male = 1,
        Female = 2
    }
}
