using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
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
builder.Services.AddScoped<BancoRepository>();
builder.Services.AddScoped<BancoBusiness>();
builder.Services.AddScoped<CalculosBusiness>();
builder.Services.AddScoped<TasaInteresRepository>();
builder.Services.AddScoped<TasaInteresBusiness>();
builder.Services.AddScoped<EndeudamientoMaximoRepository>();
builder.Services.AddScoped<EndeudamientoMaximoBusiness>();
builder.Services.AddScoped<EscenarioTasaInteresRepository>();
builder.Services.AddScoped<EscenarioTasaInteresBusiness>();
builder.Services.AddScoped<PlazosEscenariosRepository>();
builder.Services.AddScoped<PlazosEscenariosBusiness>();
builder.Services.AddScoped<SeguroBancoRepository>();
builder.Services.AddScoped<SeguroBancoBusiness>();
builder.Services.AddScoped<TipoAsalariadoRepository>();
builder.Services.AddScoped<TipoAsalariadoBusiness>();
builder.Services.AddScoped<SeguroRepository>();
builder.Services.AddScoped<SeguroBusiness>();

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
