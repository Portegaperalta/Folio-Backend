using Microsoft.AspNetCore.Identity;

namespace Folio.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
