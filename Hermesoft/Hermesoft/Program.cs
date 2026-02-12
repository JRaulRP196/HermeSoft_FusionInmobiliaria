using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using HermeSoft_Fusion.Repository.Servicios;
using HermeSoft_Fusion.Repository.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("BCCR", (sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var token = configuration["BCCR:Token"];

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);
});
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT"])
            )
        };
    });
builder.Services.AddAuthorization();
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
builder.Services.AddScoped<EscenarioTasaInteresRepository>();
builder.Services.AddScoped<PlazosEscenariosRepository>();
builder.Services.AddScoped<SeguroBancoRepository>();
builder.Services.AddScoped<TipoAsalariadoRepository>();
builder.Services.AddScoped<SeguroRepository>();
builder.Services.AddScoped<HistoricoCambiosBancariosRepository>();
builder.Services.AddScoped<Configuracion>();
builder.Services.AddScoped<TipoCambioAPIRepository>();
builder.Services.AddScoped<IndicadorEconomicoRepository>();
builder.Services.AddScoped<IndicadoresBancariosRepository>();
builder.Services.AddScoped<IndicadoresBancariosBusiness>();
builder.Services.AddScoped<TipoCambioRepository>();
builder.Services.AddScoped<TipoCambioBusiness>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioBusiness>();
builder.Services.AddScoped<PasswordService>();

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
