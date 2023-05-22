using FoodDeliverySite.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySite.Data
{
    public class ProductsContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
