using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SAE.Models.Sale
{
    public class CustomerModel : EntityModel
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Nickname { get; set; }
        public string? Identification { get; set; }
        public string? PhoneNumber { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{this.Name} {this.LastName}";
            }
        }
    }
}
