using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Restaurants") ?? "Data Source=Restaurants.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<RestaurantDb>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo {
         Title = "Restaurant API",
         Description = "Making the Pizzas you love",
         Version = "v1" });
});
// 1) define a unique string
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 2) define allowed domains, in this case "http://example.com" and "*" = all
//    domains, for testing purposes only.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
      builder =>
      {
          builder.WithOrigins(
            "http://example.com", "*");
      });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API V1");
   });
}

// 3) use the capability
app.UseCors(MyAllowSpecificOrigins);
app.MapGet("/", () => "Hello World!");
app.MapGet("/restaurants", async (RestaurantDb db) => await db.Restaurants.ToListAsync());
app.MapPost("/restaurant", async (RestaurantDb db, Restaurant restaurant) =>
{
    await db.Restaurants.AddAsync(restaurant);
    await db.SaveChangesAsync();
    return Results.Created($"/restaurant/{restaurant.Id}", restaurant);
});
app.MapGet("/restaurant/{id}", async (RestaurantDb db, int id) => await db.Restaurants.FindAsync(id));
app.MapPut("/restaurant/{id}", async (RestaurantDb db, Restaurant updaterestaurant, int id) =>
{
      var restaurant = await db.Restaurants.FindAsync(id);
      if (restaurant is null) return Results.NotFound();
      restaurant.Name = updaterestaurant.Name;
      restaurant.Description = updaterestaurant.Description;
      restaurant.Price = updaterestaurant.Price;
      await db.SaveChangesAsync();
      return Results.NoContent();
});
app.MapDelete("/restaurant/{id}", async (RestaurantDb db, int id) =>
{
   var restaurant = await db.Restaurants.FindAsync(id);
   if (restaurant is null)
   {
      return Results.NotFound();
   }
   db.Restaurants.Remove(restaurant);
   await db.SaveChangesAsync();
   return Results.Ok();
});

app.Run();
