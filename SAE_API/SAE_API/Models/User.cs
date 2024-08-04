namespace SAE_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Lastname { get; set; }
        public required string Indentification { get; set; }
        public int Age { get; set; }
        public int PhoneNumber { get; set; }
        public string? Email { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? UpdateddDate { get; set; }
        public string? UpdateddBy { get; set; }
    }
}
