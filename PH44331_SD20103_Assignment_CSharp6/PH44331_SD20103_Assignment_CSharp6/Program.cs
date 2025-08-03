using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PH44331_SD20103_Assignment_CSharp6.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace PH44331_SD20103_Assignment_CSharp6
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/denied";
                });
            builder.Services.AddDbContext<FoodDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("local")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5173") // Your Vue app's address
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowSpecificOrigin");

            // Login endpoint
            app.MapPost("/login", async (FoodDBContext _db, HttpContext http, LoginRequest loginRequest) =>
            {
                if (loginRequest == null)
                {
                    return Results.BadRequest(new { message = "Invalid request body." });
                }

                var user = await _db.Accounts
                    .FirstOrDefaultAsync(a => a.AccUsername == loginRequest.Username && a.AccPassword == loginRequest.Password);

                if (user == null)
                    return Results.Unauthorized();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.AccUsername),
                    new Claim(ClaimTypes.Role, user.AccRole)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Results.Ok(new { message = "Login successful", role = user.AccRole });
            });

            // GET: Generic CRUD for all tables (already implemented above)
            app.MapGet("/{tableName}/{id?}", async (
                string tableName,
                string? id,
                ClaimsPrincipal user,
                FoodDBContext _db) =>
            {
                var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
                var isAdmin = user.IsInRole("admin");

                switch (tableName.ToLower())
                {
                    case "food":
                        if (string.IsNullOrEmpty(id))
                            return Results.Ok(await _db.Foods.ToListAsync());
                        return Results.Ok(await _db.Foods.FirstOrDefaultAsync(x => x.Id == id));

                    case "account" when isAuthenticated && isAdmin:
                        if (string.IsNullOrEmpty(id))
                            return Results.Ok(await _db.Accounts.ToListAsync());
                        return Results.Ok(await _db.Accounts.FirstOrDefaultAsync(x => x.Id == id));

                    case "receipt" when isAuthenticated && isAdmin:
                        if (string.IsNullOrEmpty(id))
                            return Results.Ok(await _db.Receipts.ToListAsync());
                        return Results.Ok(await _db.Receipts.FirstOrDefaultAsync(x => x.Id == id));

                    case "receiptdetail" when isAuthenticated && isAdmin:
                        if (string.IsNullOrEmpty(id))
                            return Results.Ok(await _db.ReceiptDetails.ToListAsync());
                        return Results.Ok(await _db.ReceiptDetails.FirstOrDefaultAsync(x => x.Id == id));

                    default:
                        return Results.BadRequest("Invalid or unauthorized table name.");
                }
            }).RequireAuthorization();


            // POST: Generic create for all tables
            app.MapPost("/{tableName}", [Authorize(Roles = "admin")] async (FoodDBContext _db, string tableName, [FromBody] object entity) =>
            {
                switch (tableName.ToLower())
                {
                    case "food":
                        var food = System.Text.Json.JsonSerializer.Deserialize<Food>(entity.ToString());
                        if (await _db.Foods.AnyAsync(f => f.Id == food.Id))
                            return Results.Conflict("Food with this ID already exists.");
                        await _db.Foods.AddAsync(food);
                        await _db.SaveChangesAsync();
                        return Results.Created($"/food/{food.Id}", food);
                    case "account":
                        var account = System.Text.Json.JsonSerializer.Deserialize<Account>(entity.ToString());
                        if (await _db.Accounts.AnyAsync(a => a.Id == account.Id))
                            return Results.Conflict("Account with this ID already exists.");
                        await _db.Accounts.AddAsync(account);
                        await _db.SaveChangesAsync();
                        return Results.Created($"/account/{account.Id}", account);
                    case "receipt":
                        var receipt = System.Text.Json.JsonSerializer.Deserialize<Receipt>(entity.ToString());
                        if (await _db.Receipts.AnyAsync(r => r.Id == receipt.Id))
                            return Results.Conflict("Receipt with this ID already exists.");
                        await _db.Receipts.AddAsync(receipt);
                        await _db.SaveChangesAsync();
                        return Results.Created($"/receipt/{receipt.Id}", receipt);
                    case "receiptdetail":
                        var detail = System.Text.Json.JsonSerializer.Deserialize<ReceiptDetail>(entity.ToString());
                        if (await _db.ReceiptDetails.AnyAsync(rd => rd.Id == detail.Id))
                            return Results.Conflict("ReceiptDetail with this ID already exists.");
                        await _db.ReceiptDetails.AddAsync(detail);
                        await _db.SaveChangesAsync();
                        return Results.Created($"/receiptdetail/{detail.Id}", detail);
                    default:
                        return Results.BadRequest("Invalid table name");
                }
            });

            // PUT: Generic update for all tables
            app.MapPut("/{tableName}/{id}", [Authorize(Roles = "admin")] async (FoodDBContext _db, string tableName, string id, [FromBody] object entity) =>
            {
                switch (tableName.ToLower())
                {
                    case "food":
                        var updatedFood = System.Text.Json.JsonSerializer.Deserialize<Food>(entity.ToString());
                        var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == id);
                        if (food == null) return Results.NotFound();
                        food.FoodName = updatedFood.FoodName;
                        food.Available = updatedFood.Available;
                        food.Quantity = updatedFood.Quantity;
                        food.Cost = updatedFood.Cost;
                        await _db.SaveChangesAsync();
                        return Results.Ok(food);
                    case "account":
                        var updatedAccount = System.Text.Json.JsonSerializer.Deserialize<Account>(entity.ToString());
                        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
                        if (account == null) return Results.NotFound();
                        account.AccUsername = updatedAccount.AccUsername;
                        account.AccPassword = updatedAccount.AccPassword;
                        account.AccRole = updatedAccount.AccRole;
                        await _db.SaveChangesAsync();
                        return Results.Ok(account);
                    case "receipt":
                        var updatedReceipt = System.Text.Json.JsonSerializer.Deserialize<Receipt>(entity.ToString());
                        var receipt = await _db.Receipts.FirstOrDefaultAsync(r => r.Id == id);
                        if (receipt == null) return Results.NotFound();
                        receipt.AccountId = updatedReceipt.AccountId;
                        receipt.CreatedAt = updatedReceipt.CreatedAt;
                        await _db.SaveChangesAsync();
                        return Results.Ok(receipt);
                    case "receiptdetail":
                        var updatedDetail = System.Text.Json.JsonSerializer.Deserialize<ReceiptDetail>(entity.ToString());
                        var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
                        if (detail == null) return Results.NotFound();
                        detail.ReceiptId = updatedDetail.ReceiptId;
                        detail.FoodId = updatedDetail.FoodId;
                        detail.Quantity = updatedDetail.Quantity;
                        detail.TotalCost = updatedDetail.TotalCost;
                        await _db.SaveChangesAsync();
                        return Results.Ok(detail);
                    default:
                        return Results.BadRequest("Invalid table name");
                }
            });

            // DELETE: Generic delete for all tables
            app.MapDelete("/{tableName}/{id}", [Authorize(Roles = "admin")] async (FoodDBContext _db, string tableName, string id) =>
            {
                switch (tableName.ToLower())
                {
                    case "food":
                        var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == id);
                        if (food == null) return Results.NotFound();
                        _db.Foods.Remove(food);
                        await _db.SaveChangesAsync();
                        return Results.Ok($"Successfully deleted food {id}");
                    case "account":
                        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
                        if (account == null) return Results.NotFound();
                        _db.Accounts.Remove(account);
                        await _db.SaveChangesAsync();
                        return Results.Ok($"Successfully deleted account {id}");
                    case "receipt":
                        var receipt = await _db.Receipts.FirstOrDefaultAsync(r => r.Id == id);
                        if (receipt == null) return Results.NotFound();
                        _db.Receipts.Remove(receipt);
                        await _db.SaveChangesAsync();
                        return Results.Ok($"Successfully deleted receipt {id}");
                    case "receiptdetail":
                        var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
                        if (detail == null) return Results.NotFound();
                        _db.ReceiptDetails.Remove(detail);
                        await _db.SaveChangesAsync();
                        return Results.Ok($"Successfully deleted receiptdetail {id}");
                    default:
                        return Results.BadRequest("Invalid table name");
                }
            });
            app.Run();
        }
    }
}
