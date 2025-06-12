using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockFlow360.Domain.Entities;

namespace StockFlow360.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "Cashier", "Supplier" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var seedUsers = new List<(string Email, string Username, string Password, string Role)>
            {
                ("admin@stock.com", "admin", "Admin@123", "Admin"),
                ("manager@stock.com", "manager", "Manager@123", "Manager"),
                ("cashier@stock.com", "cashier", "Cashier@123", "Cashier"),
                ("supplier@stock.com", "supplier", "Supplier@123", "Supplier")
            };

            foreach (var (email, username, password, role) in seedUsers)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var newUser = new ApplicationUser
                    {
                        Email = email,
                        UserName = username,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newUser, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, role);
                    }
                }
            }
        }

        public static async Task SeedSampleProductsAndInventoryAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync()) return; // Avoid duplicate seeding

            // Seed a default supplier first
            var defaultSupplier = new Supplier
            {
                Name = "Default Supplier",
                ContactInfo = "supplier@stock.com"
                
            };

            context.Suppliers.Add(defaultSupplier);
            await context.SaveChangesAsync();

            // Use the supplier's ID for all products
            var products = new List<Product>
            {
                new() { Name = "Laptop", SKU = "LAP-001", CostPrice = 500m, SellingPrice = 700m, ReorderLevel = 5, SupplierId = defaultSupplier.Id },
                new() { Name = "Keyboard", SKU = "KEY-002", CostPrice = 15m, SellingPrice = 25m, ReorderLevel = 10, SupplierId = defaultSupplier.Id },
                new() { Name = "Mouse", SKU = "MOU-003", CostPrice = 10m, SellingPrice = 18m, ReorderLevel = 10, SupplierId = defaultSupplier.Id },
                new() { Name = "Monitor", SKU = "MON-004", CostPrice = 100m, SellingPrice = 150m, ReorderLevel = 3, SupplierId = defaultSupplier.Id },
                new() { Name = "Printer", SKU = "PRI-005", CostPrice = 80m, SellingPrice = 120m, ReorderLevel = 2, SupplierId = defaultSupplier.Id }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync(); // Save products to generate IDs

            var inventoryItems = products.Select(p => new Inventory
            {
                ProductId = p.Id,
                Quantity = 20 // Starting quantity for all
            });

            context.Inventories.AddRange(inventoryItems);
            await context.SaveChangesAsync();
        }
    }
}
