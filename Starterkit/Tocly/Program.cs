using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Repository;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(
        options => options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnection"))
    );

builder.Services.AddScoped<LoteRepository>();
builder.Services.AddScoped<LoteBusiness>();
builder.Services.AddScoped<CondominioRepository>();
builder.Services.AddScoped<CondominioBusiness>();
builder.Services.AddScoped<CoordenadasRepository>();
builder.Services.AddScoped<CoordenadasBusiness>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
