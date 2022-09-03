using Microsoft.AspNetCore.Identity;

namespace Authentication.Api.Models
{

    public class ShopUser: IdentityUser
    {
        public Guid Tenant { get; set; }
    }
}
