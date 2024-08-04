namespace SAE_API.Models.Sell
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? LastName { get; set; }
        public string? Nickname { get; set; }
        public string? Identification { get; set; }
        public string? PhoneNumber { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? UpdateddDate { get; set; }
        public string? UpdateddBy { get; set; }
    }
}
