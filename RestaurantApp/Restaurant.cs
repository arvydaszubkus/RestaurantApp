using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.Models 
{
    public class Restaurant
    {
          public int Id { get; set; }
          public string? Name { get; set; }
          public string? Description { get; set; }
          public decimal Price { get; set; }
    }

    class RestaurantDb : DbContext
    {
        public RestaurantDb(DbContextOptions options) : base(options) { }
        public DbSet<Restaurant> Restaurants { get; set; } = null!;
    }
}