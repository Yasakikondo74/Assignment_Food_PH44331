using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PH44331_SD20103_Assignment_CSharp6.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace PH44331_SD20103_Assignment_CSharp6
{
    public class PurchaseRequest
    {
        public string FoodId { get; set; }
        public int Quantity { get; set; }
    }
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

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/denied";
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Cookie.HttpOnly = true;
                });
            builder.Services.AddDbContext<FoodDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("local")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5173")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            var app = builder.Build();

            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            // Login (anyone)
            app.MapPost("/login", async (FoodDBContext _db, HttpContext http, LoginRequest loginRequest) =>
            {
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

                return Results.Ok(new
                {
                    id = user.Id,
                    accUsername = user.AccUsername,
                    accRole = user.AccRole
                });
            }).AllowAnonymous();

            // Get foods (anyone)
            app.MapGet("/food", async (FoodDBContext _db) =>
            {
                var foods = await _db.Foods.ToListAsync();
                return Results.Ok(foods);
            }).AllowAnonymous();

            app.MapGet("/food/available", async (FoodDBContext _db) =>
            {
                var availableFoods = await _db.Foods
                    .Where(f => f.Available == true || f.Quantity > 0)
                    .ToListAsync();
                return Results.Ok(availableFoods);
            }).AllowAnonymous();

            app.MapGet("/food/{id}", async (string id, FoodDBContext _db) =>
            {
                var food = await _db.Foods.FirstOrDefaultAsync(x => x.Id == id);
                if (food == null) return Results.NotFound();
                return Results.Ok(food);
            }).AllowAnonymous();

            // Account (admin only)
            app.MapGet("/account/{username}", async (string username, ClaimsPrincipal user, FoodDBContext _db) =>
            {
                if (!user.Identity.IsAuthenticated)
                    return Results.Unauthorized();
                if (user.Identity.Name != username && !user.IsInRole("admin"))
                    return Results.Forbid();

                var account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccUsername == username);
                if (account == null) return Results.NotFound();
                return Results.Ok(account);
            });

            app.MapGet("/accounts", async (FoodDBContext _db) =>
            {
                var accounts = await _db.Accounts.ToListAsync();
                return Results.Ok(accounts);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapPost("/account", async (FoodDBContext _db, [FromBody] Account account) =>
            {
                if (await _db.Accounts.AnyAsync(a => a.Id == account.Id))
                    return Results.Conflict("Account with this ID already exists.");
                await _db.Accounts.AddAsync(account);
                await _db.SaveChangesAsync();
                return Results.Created($"/account/{account.Id}", account);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapPut("/account/{id}", async (FoodDBContext _db, string id, Account updatedAccount) =>
            {
                var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
                if (account == null) return Results.NotFound();
                account.AccUsername = updatedAccount.AccUsername;
                account.AccPassword = updatedAccount.AccPassword;
                account.AccRole = updatedAccount.AccRole;
                await _db.SaveChangesAsync();
                return Results.Ok(account);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapDelete("/account/{id}", async (FoodDBContext _db, string id) =>
            {
                var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
                if (account == null) return Results.NotFound();
                _db.Accounts.Remove(account);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted account {id}");
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            // Food CRUD (admin only for changes)
            app.MapPost("/food", async (FoodDBContext _db, [FromBody] Food food) =>
            {
                if (food.Id.IsNullOrEmpty())
                    food.Id = Guid.NewGuid().ToString();
                await _db.Foods.AddAsync(food);
                await _db.SaveChangesAsync();
                return Results.Created($"/food/{food.Id}", food);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapPut("/food/{id}", async (FoodDBContext _db, string id, Food updatedFood) =>
            {
                var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == id);
                if (food == null) return Results.NotFound();
                food.FoodName = updatedFood.FoodName;
                food.Available = updatedFood.Available;
                food.Quantity = updatedFood.Quantity;
                food.Cost = updatedFood.Cost;
                food.ImageLink = updatedFood.ImageLink;
                await _db.SaveChangesAsync();
                return Results.Ok(food);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapDelete("/food/{id}", async (FoodDBContext _db, string id) =>
            {
                var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == id);
                if (food == null) return Results.NotFound();
                _db.Foods.Remove(food);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted food {id}");
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            // Receipts & ReceiptDetails
            app.MapGet("/receipt", async (FoodDBContext _db) =>
            {
                var receipts = await _db.Receipts.ToListAsync();
                return Results.Ok(receipts);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin,user" });

            app.MapGet("/receipt/{id}", async (string id, FoodDBContext _db) =>
            {
                var receipt = await _db.Receipts.FirstOrDefaultAsync(x => x.Id == id);
                if (receipt == null) return Results.NotFound();
                return Results.Ok(receipt);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin,user" });

            app.MapPost("/receipt", async (FoodDBContext _db, [FromBody] Receipt receipt) =>
            {
                if (await _db.Receipts.AnyAsync(r => r.Id == receipt.Id))
                    return Results.Conflict("Receipt with this ID already exists.");
                await _db.Receipts.AddAsync(receipt);
                await _db.SaveChangesAsync();
                return Results.Created($"/receipt/{receipt.Id}", receipt);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin,user" });

            app.MapPut("/receipt/{id}", async (FoodDBContext _db, string id, Receipt updatedReceipt) =>
            {
                var receipt = await _db.Receipts.FirstOrDefaultAsync(r => r.Id == id);
                if (receipt == null) return Results.NotFound();
                receipt.AccountId = updatedReceipt.AccountId;
                receipt.CreatedAt = updatedReceipt.CreatedAt;
                await _db.SaveChangesAsync();
                return Results.Ok(receipt);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapDelete("/receipt/{id}", async (FoodDBContext _db, string id) =>
            {
                var receipt = await _db.Receipts.FirstOrDefaultAsync(r => r.Id == id);
                if (receipt == null) return Results.NotFound();
                _db.Receipts.Remove(receipt);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted receipt {id}");
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapGet("/receiptdetail", async (FoodDBContext _db) =>
            {
                var details = await _db.ReceiptDetails.ToListAsync();
                return Results.Ok(details);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin,user" });

            app.MapGet("/receiptdetail/{id}", async (string id, FoodDBContext _db) =>
            {
                var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(x => x.Id == id);
                if (detail == null) return Results.NotFound();
                return Results.Ok(detail);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin,user" });

            app.MapPost("/receiptdetail", async (FoodDBContext _db, [FromBody] ReceiptDetail detail) =>
            {
                if (await _db.ReceiptDetails.AnyAsync(rd => rd.Id == detail.Id))
                    return Results.Conflict("ReceiptDetail with this ID already exists.");
                await _db.ReceiptDetails.AddAsync(detail);
                await _db.SaveChangesAsync();
                return Results.Created($"/receiptdetail/{detail.Id}", detail);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin,user" });

            app.MapPut("/receiptdetail/{id}", async (FoodDBContext _db, string id, ReceiptDetail updatedDetail) =>
            {
                var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
                if (detail == null) return Results.NotFound();
                detail.ReceiptId = updatedDetail.ReceiptId;
                detail.FoodId = updatedDetail.FoodId;
                detail.Quantity = updatedDetail.Quantity;
                detail.TotalCost = updatedDetail.TotalCost;
                await _db.SaveChangesAsync();
                return Results.Ok(detail);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.MapDelete("/receiptdetail/{id}", async (FoodDBContext _db, string id) =>
            {
                var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
                if (detail == null) return Results.NotFound();
                _db.ReceiptDetails.Remove(detail);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted receiptdetail {id}");
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

            app.Run();
        }
    }
}
