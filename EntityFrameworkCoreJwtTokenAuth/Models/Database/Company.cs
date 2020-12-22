using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EntityFrameworkCoreJwtTokenAuth.Models.Global;

namespace EntityFrameworkCoreJwtTokenAuth.Models.Database
{
    public class Company
    {
        [Key, DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public string PhoneNumber { get; set; }
        public string EMail { get; set; }
        public int DefaultStudentPassword { get; set; }
        public int WeeklyProgramVersion { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int Balance { get; set; }
        public Timestamps Timestamps { get; set; }
    }
}
