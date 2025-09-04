using Microsoft.EntityFrameworkCore;

namespace ShopOnline.Api.Data
{
    public class ShopOnlineDbContext:DbContext
    {
        public ShopOnlineDbContext(DbContextOptions<ShopOnlineDbContext> options) : base(options)
        {

        }
    }
}
