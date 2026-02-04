using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnection"))
);


builder.Services.AddScoped<LoteRepository>();
builder.Services.AddScoped<LoteBusiness>();
builder.Services.AddScoped<CondominioRepository>();
builder.Services.AddScoped<CondominioBusiness>();
builder.Services.AddScoped<CoordenadasRepository>();
builder.Services.AddScoped<CoordenadasBusiness>();

builder.Services.AddScoped<IndicadorBancarioRepository>();
builder.Services.AddScoped<BccrService>();
builder.Services.AddScoped<IndicadorBancarioBusiness>();
builder.Services.AddScoped<CalculosBusiness>();



builder.Services.AddHostedService<BccrHostedService>();


builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
