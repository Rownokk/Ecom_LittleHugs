using Microsoft.EntityFrameworkCore;

namespace Ecom_LittleHugs.Models
{
    public class myContext : DbContext
    {
        public myContext(DbContextOptions<myContext> options) : base(options) { }
    }
}
