using BikeStoresLINQTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace BikeStoresLINQTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new Data.ApplicationDbContext();
            while (true)
            {
                Console.WriteLine("___________BIKE STORE __________");
                Console.WriteLine("1. List customers with emails");
                Console.WriteLine("2. Orders by specific staff");
                Console.WriteLine("3. Mountain Bikes products");
                Console.WriteLine("4. Orders count per store");
                Console.WriteLine("5. Unshipped orders");
                Console.WriteLine("6. Customers with order count");
                Console.WriteLine("7. Never ordered products");
                Console.WriteLine("8. Low stock products (<5)");
                Console.WriteLine("9. First product");
                Console.WriteLine("10. Products by model year");
                Console.WriteLine("11. Products with order frequency");
                Console.WriteLine("12. Products count in category");
                Console.WriteLine("13. Average product price");
                Console.WriteLine("14. Product by ID");
                Console.WriteLine("15. Products ordered in quantity > 3");
                Console.WriteLine("16. Staff with order count");
                Console.WriteLine("17. Active staff members");
                Console.WriteLine("18. Products with brand and category");
                Console.WriteLine("19. Completed orders");
                Console.WriteLine("20. Products with total sold quantity");

                Console.Write("Select an option (1-20) : ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        {
                            Console.WriteLine("List all customers' first and last names along with their email addresses. ");
                            var customers = context.Customers
                                .Select(e => new {
                                    e.FirstName,
                                    e.LastName,
                                    e.Email
                                })
                                .Take(10)
                                .ToList();
                            foreach (var item in customers)
                            {
                                Console.WriteLine($"{item.FirstName} {item.LastName} - {item.Email}");
                            }

                        }
                        break;
                    case "2":
                        {
                            Console.WriteLine(" Retrieve all orders processed by a specific staff member (e.g., staff_id = 3)");
                            var staffOrders = context.Orders
                                .Where(e => e.StaffId == 3)
                                .Include(e => e.Staff)
                                .Take(10)
                                .ToList();
                            foreach (var item in staffOrders)
                            {
                                Console.WriteLine($"Order {item.OrderId} - Date: {item.OrderDate:yyyy-MM-dd} - Staff: {item.Staff?.FirstName}");
                            }
                        }
                        break;
                    case "3":
                        {
                            Console.WriteLine("Get all products that belong to a category named \"Mountain Bikes\"");
                            var mountainBikes = context.Products
                                .Include(e => e.Category)
                                .Where(e => e.Category.CategoryName == "Mountain Bikes")
                                .Take(10)
                                .ToList();
                            foreach (var item in mountainBikes)
                            {
                                Console.WriteLine($"{item.ProductName} - ${item.ListPrice}");
                            }
                        }
                        break;
                    case "4":
                        {
                            Console.WriteLine("Count the total number of orders per store.");
                            var ordersPerStore = context.Stores
                                .GroupBy(e => e.StoreId)
                                .Select(e => new {
                                    storeId = e.Key,
                                    OrdersCount = e.Count()
                                })
                                .ToList();
                            foreach (var item in ordersPerStore)
                            {
                                Console.WriteLine($"Store {item.storeId}: {item.OrdersCount} orders");
                            }
                        }
                        break;
                    case "5":
                        {
                            Console.WriteLine("List all orders that have not been shipped yet (i.e., shipped_date is null).");
                            var unshippedOrders = context.Orders
                                .Where(e => e.ShippedDate == null)
                                .Take(10)
                                .ToList();
                            foreach (var item in unshippedOrders)
                            {
                                Console.WriteLine($"Order {item.OrderId} - Required: {item.RequiredDate:yyyy-MM-dd}");
                            }
                        }
                        break;
                    case "6":
                        {
                            Console.WriteLine("Display each customer’s full name and the number of orders they have placed.");
                            var customerOrderCounts = context.Customers
                                .Select(e => new {
                                    FullName = e.FirstName + " " + e.LastName,
                                    e.Email,
                                    OrderCount = e.Orders.Count()
                                })
                                .OrderByDescending(e => e.OrderCount)
                                .Take(10)
                                .ToList();
                            foreach (var item in customerOrderCounts)
                            {
                                Console.WriteLine($"{item.FullName}: {item.OrderCount} orders ({item.Email})");
                            }
                        }
                        break;
                    case "7":
                        {
                            Console.WriteLine(" List all products that have never been ordered (not found in order_items). ");
                            var neverOrderedProducts = context.Products
                                .Where(p => !context.OrderItems.Any(oi => oi.ProductId == p.ProductId))
                                .Take(10)
                                .ToList();
                            foreach (var item in neverOrderedProducts)
                            {
                                Console.WriteLine($"{item.ProductName} - ${item.ListPrice}");
                            }
                        }
                        break;
                    case "8":
                        {
                            Console.WriteLine("Display products that have a quantity of less than 5 in any store stock");
                            var lowStockProducts = context.Stocks
                                .Include(e => e.Product)
                                .Include(e => e.Store)
                                .Where(e => e.Quantity < 5)
                                .Select(e => new
                                {
                                    ProductName = e.Product.ProductName,
                                    StoreName = e.Store.StoreName,
                                    Quantity = e.Quantity
                                })
                                .Take(10)
                                .ToList();
                            foreach (var item in lowStockProducts)
                            {
                                Console.WriteLine($"{item.ProductName} in {item.StoreName}: {item.Quantity} units");
                            }

                        }
                        break;
                    case "9":
                        {
                            Console.WriteLine(" Retrieve the first product from the products table.");
                            var firstProduct = context.Products
                                .Include(e => e.Brand)
                                .Include(e => e.Category)
                                .FirstOrDefault();
                            Console.WriteLine("First product in database:");
                            if (firstProduct != null)
                            {
                                Console.WriteLine($"ID: {firstProduct.ProductId}");
                                Console.WriteLine($"Name: {firstProduct.ProductName}");
                                Console.WriteLine($"Brand: {firstProduct.Brand?.BrandName}");
                                Console.WriteLine($"Category: {firstProduct.Category?.CategoryName}");
                                Console.WriteLine($"Price: ${firstProduct.ListPrice}");
                                Console.WriteLine($"Model Year: {firstProduct.ModelYear}");
                            }
                        }
                        break;
                    case "10":
                        {
                            Console.WriteLine(" Retrieve all products from the products table with a certain model year. ");
                            Console.Write("Enter model year to search (default 2018): ");
                            var yearInput = Console.ReadLine();
                            short year = string.IsNullOrEmpty(yearInput) ? (short)2018 : short.Parse(yearInput);
                            var productsByYear = context.Products
                                    .Where(p => p.ModelYear == year)
                                    .Take(10)
                                    .ToList();
                            Console.WriteLine($"\nProducts from model year {year}:");
                            Console.WriteLine($"Total: {context.Products.Count(p => p.ModelYear == year)} products");

                            foreach (var item in productsByYear)
                            {
                                Console.WriteLine($"{item.ProductName} - ${item.ListPrice}");
                            }
                        }
                            break;
                    case "11":
                        {
                            Console.WriteLine("Display each product with the number of times it was ordered");
                            var productsOrderCount = context.Products
                               .Select(e => new
                               {
                                   e.ProductName,
                                   OrderCount = e.OrderItems.Count,
                                   TotalQuantity = e.OrderItems.Sum(oi => oi.Quantity)
                               })
                               .Where(e => e.TotalQuantity > 0)
                               .OrderByDescending(e => e.TotalQuantity)
                               .Take(10)
                               .ToList();

                            Console.WriteLine("Top 10 products by order quantity:");
                            foreach (var item in productsOrderCount)
                            {
                                Console.WriteLine($"{item.ProductName} - Ordered {item.OrderCount} times, Total Quantity: {item.TotalQuantity}");
                            }
                        }
                        break;
                    case "12":
                        {
                            Console.WriteLine(" Count the number of products in a specific category");
                            var categories = context.Categories.ToList();
                            Console.WriteLine("Available categories:");
                            foreach (var item in categories)
                            {
                                Console.WriteLine($"- {item.CategoryName}");
                            }
                            Console.Write("\nEnter category name (default: Mountain Bikes): ");
                            string categoryName = Console.ReadLine();
                            if (string.IsNullOrEmpty(categoryName))
                                categoryName = "Mountain Bikes";

                            var productCount = context.Products
                                .Count(e => e.Category.CategoryName == categoryName);
                            Console.WriteLine($"\nNumber of products in '{categoryName}': {productCount}");
                        }
                        break;
                    case "13":
                        {
                            Console.WriteLine("Calculate the average list price of products. ");
                            var avgPrice = context.Products.Average(p => p.ListPrice);
                            var minPrice = context.Products.Min(p => p.ListPrice);
                            var maxPrice = context.Products.Max(p => p.ListPrice);

                            Console.WriteLine("Product Price Analysis:");
                            Console.WriteLine($"Average Price: ${avgPrice:F2}");
                            Console.WriteLine($"Minimum Price: ${minPrice:F2}");
                            Console.WriteLine($"Maximum Price: ${maxPrice:F2}");
                            Console.WriteLine($"Price Range: ${maxPrice - minPrice:F2}");
                            break;
                        }
                        break;
                    case "14":
                        {
                            Console.WriteLine("Retrieve a specific product from the products table by ID");
                            Console.Write("Enter product ID to search (default 5): ");
                            string input14 = Console.ReadLine();
                            int productId = string.IsNullOrEmpty(input14) ? 5 : int.Parse(input14);

                            var productById = context.Products
                                .Include(e => e.Brand)
                                .Include(e => e.Category)
                                .FirstOrDefault(e => e.ProductId == productId);
                            if (productById != null)
                            {
                                Console.WriteLine($"Name: {productById.ProductName}");
                                Console.WriteLine($"Brand: {productById.Brand?.BrandName}");
                                Console.WriteLine($"Category: {productById.Category?.CategoryName}");
                                Console.WriteLine($"Price: ${productById.ListPrice}");
                                Console.WriteLine($"Model Year: {productById.ModelYear}");

                                var stock = context.Stocks
                                    .Include(s => s.Store)
                                    .Where(s => s.ProductId == productId)
                                    .ToList();

                                Console.WriteLine("\nStock in stores:");
                                foreach (var s in stock)
                                {
                                    Console.WriteLine($"  {s.Store?.StoreName}: {s.Quantity} units");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Product not found!");
                            }
                        }
                        break;
                    case "15":
                        {
                            Console.WriteLine(" List all products that were ordered with a quantity greater than 3 in any order.");
                            var highQuantityProducts = context.OrderItems
                                .Include(oi => oi.Product)
                                .Where(oi => oi.Quantity > 3)
                                .Select(oi => oi.Product)
                                .Distinct()
                                .Take(10)
                                .ToList();
                            foreach (var item in highQuantityProducts)
                            {
                                var maxQuantity = context.OrderItems
                                    .Where(e => e.ProductId == item.ProductId)
                                    .Max(e => e.Quantity);

                                Console.WriteLine($"{item.ProductName} (max ordered: {maxQuantity} units)");
                            }
                        }
                        break;
                    case "16":
                        {
                            Console.WriteLine(" Display each staff member’s name and how many orders they processed.");
                            var staffOrderCount = context.Staffs
                                .Select(s => new
                                {
                                    Name = s.FirstName + " " + s.LastName,
                                    s.Email,
                                    s.Phone,
                                    OrderCount = s.Orders.Count,
                                    Store = s.Store.StoreName
                                })
                                .OrderByDescending(s => s.OrderCount)
                                .ToList();
                            foreach (var item in staffOrderCount)
                            {
                                Console.WriteLine($"{item.Name}: {item.OrderCount} orders | Store: {item.Store} | {item.Phone}");
                            }
                        }
                        break;
                    case "17":
                        {
                            Console.WriteLine("- List active staff members only (active = true) along with their phone numbers.");
                            var activeStaff = context.Staffs
                                .Include(e => e.Store)
                                .Where(e => e.Active == 1)
                                .Select(e => new
                                {
                                    e.FirstName,
                                    e.LastName,
                                    e.Phone,
                                    e.Email,
                                    Store = e.Store.StoreName,
                                    Manager = e.Manager != null ? e.Manager.FirstName + " " + e.Manager.LastName : "None"
                                })
                                .ToList();
                            foreach (var item in activeStaff)
                            {
                                Console.WriteLine($"{item.FirstName} {item.LastName}");
                                Console.WriteLine($"  Phone: {item.Phone} | Email: {item.Email}");
                                Console.WriteLine($"  Store: {item.Store} | Manager: {item.Manager}");
                                Console.WriteLine();
                            }
                        }
                        break;
                    case "18":
                        Console.WriteLine(" List all products with their brand name and category name. ");
                        var productsWithDetails = context.Products
                               .Include(e => e.Brand)
                               .Include(e => e.Category)
                               .Select(e => new
                               {
                                   e.ProductName,
                                   Brand = e.Brand.BrandName,
                                   Category = e.Category.CategoryName,
                                   e.ListPrice,
                                   e.ModelYear
                               })
                               .Take(10)
                               .ToList();
                        foreach (var item in productsWithDetails)
                        {
                            Console.WriteLine($"{item.ProductName}");
                            Console.WriteLine($"  Brand: {item.Brand} | Category: {item.Category}");
                            Console.WriteLine($"  Price: ${item.ListPrice} | Year: {item.ModelYear}");
                            Console.WriteLine();
                        }
                        break;
                    case "19":
                        {
                            Console.WriteLine(" Retrieve orders that are completed. ");
                            var completedOrders = context.Orders
                                .Include(o => o.Customer)
                                .Include(o => o.Staff)
                                .Where(o => o.ShippedDate != null)
                                .OrderByDescending(o => o.ShippedDate)
                                .Take(10)
                                .ToList();
                            foreach (var order in completedOrders)
                            {
                                
                                if (order.ShippedDate.HasValue)
                                {
                                    
                                    var processingTime = order.ShippedDate.Value.DayNumber - order.OrderDate.DayNumber;
                                    Console.WriteLine($"Order #{order.OrderId}");
                                    Console.WriteLine($"  Customer: {order.Customer?.FirstName} {order.Customer?.LastName}");
                                    Console.WriteLine($"  Staff: {order.Staff?.FirstName}");
                                    Console.WriteLine($"  Ordered: {order.OrderDate:yyyy-MM-dd} | Shipped: {order.ShippedDate:yyyy-MM-dd}");
                                    Console.WriteLine($"  Processing Time: {processingTime} days");
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine($"Order #{order.OrderId} has not been shipped yet.");
                                }
                            }
                        }
                        break;
                    case "20":
                        {
                            Console.WriteLine(" List each product with the total quantity sold (sum of quantity from order_items).");
                            var productsSold = context.Products
                                .Select(p => new
                                {
                                    p.ProductName,
                                    TotalSold = p.OrderItems.Sum(oi => oi.Quantity),
                                    TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * (oi.ListPrice - oi.Discount)),
                                    TimesOrdered = p.OrderItems.Count
                                })
                                .Where(p => p.TotalSold > 0)
                                .OrderByDescending(p => p.TotalSold)
                                .Take(10)
                                .ToList();
                            foreach (var p in productsSold)
                            {
                                Console.WriteLine($"{p.ProductName}");
                                Console.WriteLine($"  Sold: {p.TotalSold} units | Orders: {p.TimesOrdered} times");
                                Console.WriteLine($"  Revenue: ${p.TotalRevenue:F2}");
                                Console.WriteLine();
                            }

                        } break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;


                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();



            }
        }
    }
}
