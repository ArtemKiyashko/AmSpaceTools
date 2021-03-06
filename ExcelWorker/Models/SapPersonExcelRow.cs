﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPPlus.Core.Extensions;

namespace ExcelWorker.Models
{
    public class SapPersonExcelRow
    {
        [ExcelTableColumn("Name")]
        [Required]
        public string Name { get; set; }

        [ExcelTableColumn("Surname")]
        [Required]
        public string Surname { get; set; }

        [Required]
        [ExcelTableColumn("IdentityNumber")]
        public string IdentityNumber { get; set; }

        [ExcelTableColumn("Mpk")]
        [Required]
        public long? Mpk { get; set; }

        [ExcelTableColumn("Email")]
        public string Email { get; set; }

        [ExcelTableColumn("Phone")]
        public string Phone { get; set; }

        [ExcelTableColumn("DateOfBirth")]
        public DateTime? BirthDate { get; set; }

        [ExcelTableColumn("Nationality")]
        [Required]
        public string Nationality { get; set; }

        [ExcelTableColumn("Country")]
        [Required]
        public string Country { get; set; }

        [ExcelTableColumn("Sex")]
        [Required]
        public Sex Sex { get; set; }

        [ExcelTableColumn("ManagerId")]
        public string ManagerId { get; set; }

        [ExcelTableColumn("ContractNumber")]
        [Required]
        public int ContractNumber { get; set; }

        [ExcelTableColumn("Position")]
        [Required]
        public string Position { get; set; }

        [ExcelTableColumn("Level")]
        [Required]
        public int Level { get; set; }

        [ExcelTableColumn("StartDate")]
        [Required]
        public DateTime ContractStartDate { get; set; }

        [ExcelTableColumn("EndDate")]
        public DateTime? ContractEndDate { get; set; }

        [ExcelTableColumn("Status")]
        [Required]
        public ContractStatus Status { get; set; }

        [ExcelTableColumn("AmRest Manager Name")]
        public string AmRestManagerName { get; set; }
    }

    public enum ContractStatus
    {
        ACTIVE,
        SUSPENDED,
        TERMINATED,
    }
    public enum Sex
    {
        MALE,
        FEMALE
    }
}
