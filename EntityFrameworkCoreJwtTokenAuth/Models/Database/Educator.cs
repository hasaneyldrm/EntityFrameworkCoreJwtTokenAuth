using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EntityFrameworkCoreJwtTokenAuth.Models.Global;

namespace EntityFrameworkCoreJwtTokenAuth.Models.Database
{
    public class Educator
    {
        [Key, DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int CompanyId { get; set; }
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int Password { get; set; }
        public bool Gender { get; set; }
        public string AdminNote { get; set; }
        //[NotMapped]
        //public List<EducatorLicense> EducatorLicense { get; set; }
        public bool Status { get; set; }
        public Timestamps Timestamps { get; set; }
    }
}
