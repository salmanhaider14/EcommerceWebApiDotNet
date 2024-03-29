namespace DotNetEcommerceAPI.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class IdentityExtensions
{
    public static async Task RegisterAsync(this HttpContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

        if (!userData.ContainsKey("email") || !userData.ContainsKey("password"))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Email and password are required.");
            return;
        }

        var email = userData["email"];
        var password = userData["password"];

        var user = new IdentityUser { UserName = email, Email = email };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Customer");

            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("User registered successfully!");
        }
        else
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Failed to register user. Please try again.");
        }
    }
    public static async Task CreateAdminUserAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var email = "ecomadmin@gmail.com";
            var pass = "Abc@12345";

            var user = new IdentityUser();
            user.UserName = email;
            user.Email = email;

            await userManager.CreateAsync(user, pass);
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

