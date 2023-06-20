using Microsoft.Extensions.Configuration;
using PaimonShopApi.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase"));
});

//Authentication cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Token";
        options.Cookie.Path = "/"; // Đường dẫn của cookie (gốc)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian hết hạn của cookie (30 phút)
        options.SlidingExpiration = true; // Kích hoạt cơ chế trượt thời gian hết hạn
         options.Cookie.SameSite = SameSiteMode.None; // Add this line
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Add this line to enforce HTTPS    
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Authorization"); // Add this line; // Cho phép gửi cookie
});

app.UseHttpsRedirection();

app.UseAuthentication(); // Thêm middleware xác thực

app.UseAuthorization();

app.MapControllers();

app.Run();