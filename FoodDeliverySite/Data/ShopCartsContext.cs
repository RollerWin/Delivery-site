using FoodDeliverySite.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySite.Data
{
    public class ShopCartsContext : DbContext
    {
        public DbSet<ShopCart> ShopCarts { get; set; }

        public ShopCartsContext(DbContextOptions<ShopCartsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
