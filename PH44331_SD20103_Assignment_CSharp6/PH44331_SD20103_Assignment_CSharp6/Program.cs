using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
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

            // Configure the HTTP request pipeline.
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            // Dedicated login endpoint
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

            // Dedicated: Get a single account by username
            app.MapGet("/account/{username}", async (string username, ClaimsPrincipal user, FoodDBContext _db) =>
            {
                if (!user.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
                }
                if (user.Identity.Name != username && !user.IsInRole("admin"))
                {
                    return Results.Forbid();
                }
                var account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccUsername == username);
                if (account == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(account);
            }).RequireAuthorization();

            // Dedicated: Get all foods
            app.MapGet("/food", async (FoodDBContext _db) =>
            {
                var foods = await _db.Foods.ToListAsync();
                return Results.Ok(foods);
            }).RequireAuthorization();

            // Dedicated: Get food by id
            app.MapGet("/food/{id}", async (string id, FoodDBContext _db) =>
            {
                var food = await _db.Foods.FirstOrDefaultAsync(x => x.Id == id);
                if (food == null) return Results.NotFound();
                return Results.Ok(food);
            }).RequireAuthorization();

            // Dedicated: Get all receipts (admin only)
            app.MapGet("/receipt", [Authorize(Roles = "admin     ")] async (FoodDBContext _db) =>
            {
                var receipts = await _db.Receipts.ToListAsync();
                return Results.Ok(receipts);
            });

            // Dedicated: Get receipt by id (admin only)
            app.MapGet("/receipt/{id}", [Authorize(Roles = "admin     ")] async (string id, FoodDBContext _db) =>
            {
                var receipt = await _db.Receipts.FirstOrDefaultAsync(x => x.Id == id);
                if (receipt == null) return Results.NotFound();
                return Results.Ok(receipt);
            });

            // Dedicated: Get all receipt details (admin only)
            app.MapGet("/receiptdetail", [Authorize(Roles = "admin     ")] async (FoodDBContext _db) =>
            {
                var details = await _db.ReceiptDetails.ToListAsync();
                return Results.Ok(details);
            });

            // Dedicated: Get receipt detail by id (admin only)
            app.MapGet("/receiptdetail/{id}", [Authorize(Roles = "admin     ")] async (string id, FoodDBContext _db) =>
            {
                var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(x => x.Id == id);
                if (detail == null) return Results.NotFound();
                return Results.Ok(detail);
            });

            // Dedicated: Get all accounts (admin only)
            app.MapGet("/accounts", [Authorize(Roles = "admin     ")] async (FoodDBContext _db) =>
            {
                var accounts = await _db.Accounts.ToListAsync();
                return Results.Ok(accounts);
            });

            // Dedicated: Create food (admin only)
            app.MapPost("/food", async (FoodDBContext _db, [FromBody] Food food) =>
            {
                if (food.Id.IsNullOrEmpty())
                {
                    food.Id = Guid.NewGuid().ToString();
                }
                await _db.Foods.AddAsync(food);
                await _db.SaveChangesAsync();
                return Results.Created($"/food/{food.Id}", food);
            });

            // Dedicated: Create account (admin only)
            app.MapPost("/account", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, [FromBody] Account account) =>
            {
                if (await _db.Accounts.AnyAsync(a => a.Id == account.Id))
                    return Results.Conflict("Account with this ID already exists.");
                await _db.Accounts.AddAsync(account);
                await _db.SaveChangesAsync();
                return Results.Created($"/account/{account.Id}", account);
            });

            // Dedicated: Create receipt (admin only)
            app.MapPost("/receipt", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, [FromBody] Receipt receipt) =>
            {
                if (await _db.Receipts.AnyAsync(r => r.Id == receipt.Id))
                    return Results.Conflict("Receipt with this ID already exists.");
                await _db.Receipts.AddAsync(receipt);
                await _db.SaveChangesAsync();
                return Results.Created($"/receipt/{receipt.Id}", receipt);
            });

            // Dedicated: Create receipt detail (admin only)
            app.MapPost("/receiptdetail", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, [FromBody] ReceiptDetail detail) =>
            {
                if (await _db.ReceiptDetails.AnyAsync(rd => rd.Id == detail.Id))
                    return Results.Conflict("ReceiptDetail with this ID already exists.");
                await _db.ReceiptDetails.AddAsync(detail);
                await _db.SaveChangesAsync();
                return Results.Created($"/receiptdetail/{detail.Id}", detail);
            });

            // Dedicated: Update food (admin only)
            app.MapPut("/food/{id}", async (FoodDBContext _db, string id, Food updatedFood) =>
            {
                var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == id);
                if (food == null) return Results.NotFound();
                food.FoodName = updatedFood.FoodName;
                food.Available = updatedFood.Available;
                food.Quantity = updatedFood.Quantity;
                food.Cost = updatedFood.Cost;
                await _db.SaveChangesAsync();
                return Results.Ok(food);
            });

            // Dedicated: Update account (admin only)
            app.MapPut("/account/{id}", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, string id, Account updatedAccount) =>
            {
                var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
                if (account == null) return Results.NotFound();
                account.AccUsername = updatedAccount.AccUsername;
                account.AccPassword = updatedAccount.AccPassword;
                account.AccRole = updatedAccount.AccRole;
                await _db.SaveChangesAsync();
                return Results.Ok(account);
            });

            // Dedicated: Update receipt (admin only)
            app.MapPut("/receipt/{id}", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, string id, Receipt updatedReceipt) =>
            {
                var receipt = await _db.Receipts.FirstOrDefaultAsync(r => r.Id == id);
                if (receipt == null) return Results.NotFound();
                receipt.AccountId = updatedReceipt.AccountId;
                receipt.CreatedAt = updatedReceipt.CreatedAt;
                await _db.SaveChangesAsync();
                return Results.Ok(receipt);
            });

            // Dedicated: Update receipt detail (admin only)
            app.MapPut("/receiptdetail/{id}", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, string id, ReceiptDetail updatedDetail) =>
            {
                var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
                if (detail == null) return Results.NotFound();
                detail.ReceiptId = updatedDetail.ReceiptId;
                detail.FoodId = updatedDetail.FoodId;
                detail.Quantity = updatedDetail.Quantity;
                detail.TotalCost = updatedDetail.TotalCost;
                await _db.SaveChangesAsync();
                return Results.Ok(detail);
            });

            // Dedicated: Delete food (admin only)
            app.MapDelete("/food/{id}", async (FoodDBContext _db, string id) =>
            {
                var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == id);
                if (food == null) return Results.NotFound();
                _db.Foods.Remove(food);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted food {id}");
            });

            // Dedicated: Delete account (admin only)
            app.MapDelete("/account/{id}", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, string id) =>
            {
                var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
                if (account == null) return Results.NotFound();
                _db.Accounts.Remove(account);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted account {id}");
            });

            // Dedicated: Delete receipt (admin only)
            app.MapDelete("/receipt/{id}", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, string id) =>
            {
                var receipt = await _db.Receipts.FirstOrDefaultAsync(r => r.Id == id);
                if (receipt == null) return Results.NotFound();
                _db.Receipts.Remove(receipt);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted receipt {id}");
            });

            // Dedicated: Delete receipt detail (admin only)
            app.MapDelete("/receiptdetail/{id}", [Authorize(Roles = "admin     ")] async (FoodDBContext _db, string id) =>
            {
                var detail = await _db.ReceiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
                if (detail == null) return Results.NotFound();
                _db.ReceiptDetails.Remove(detail);
                await _db.SaveChangesAsync();
                return Results.Ok($"Successfully deleted receiptdetail {id}");
            });

            app.Run();
        }
    }
}
