namespace Services.DTOs.Auth
{
    public class CurrentUserResponse
    {
        public string? AccountId { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? KitchenId { get; set; }
    }
}
