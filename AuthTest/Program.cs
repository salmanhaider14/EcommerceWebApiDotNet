
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppContext>();

var app = builder.Build();

app.MapPost("/api/register", async (HttpContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) =>
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
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Customer", "Admin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
using (var scope = app.Services.CreateScope())
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

app.Run();
public record RegistrationModel(string Email, string Password);