using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EntityFrameworkCoreJwtTokenAuth.Models.Global;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCoreJwtTokenAuth.Models.Database
{
    public class Student
    {
        [Key, DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public string PublicId { get; set; }
        [JsonIgnore]
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int Password { get; set; }
        public bool Gender { get; set; }
        public string LicenseType { get; set; }
        public Classification Class { get; set; }
        public SessionModel Sessions { get; set; }
        public int Debt { get; set; }
        public ExamModel Exam { get; set; }
        public string AdminNote { get; set; }
        public bool Status { get; set; }
        public Timestamps Timestamps { get; set; }
    }
    [Owned]
    public class Classification
    {
        public int Group { get; set; }
        public int Period { get; set; }
        public string Branch { get; set; }

    }

    [Owned]
    public class SessionModel
    {
        public int Remaining { get; set; }
        public int Selectable { get; set; }
        public int? Past { get; set; }
    }

    [Owned]
    public class ExamModel
    {
        public DateTime? Soonest { get; set; }
        public DateTime? Exact { get; set; }

    }
}
