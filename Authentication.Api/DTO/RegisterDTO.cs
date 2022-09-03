namespace Authentication.Api.DTO
{
    public class RegisterDTO
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid Tenant { get; set; }
    }
}
