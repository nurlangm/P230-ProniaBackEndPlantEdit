using Microsoft.AspNetCore.Identity;

namespace P230_Pronia.Entities
{
    public class User:IdentityUser
    {
        public string FullName { get; set; }
    }
}
