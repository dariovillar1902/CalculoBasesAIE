using CalculoBasesAIE.Models;
using CalculoBasesAIE.Repositories.BaseHormigonRepository;
using CalculoBasesAIE.Services.BaseHormigonIOService;
using CalculoBasesAIE.Services.BaseHormigonService;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// CORS para frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(
                        "http://localhost:5173",
                        "https://calculo-bases-aie.vercel.app")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
});

// Conversión de DATABASE_URL a cadena de conexión PostgreSQL
static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var port = uri.Port == -1 ? 5432 : uri.Port;

    return $"Host={uri.Host};Port={port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=true";
}

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CalculoBasesAIE API", Version = "v1" });
});

builder.Services.AddScoped<IBaseHormigonRepository, BaseHormigonRepository>();
builder.Services.AddScoped<IBaseHormigonService, BaseHormigonService>();
builder.Services.AddScoped<IBaseHormigonIOService, BaseHormigonIOService>();
builder.Services.AddScoped<BaseHormigonService>();
builder.Services.AddScoped<BaseHormigonIOService>();

// Configuración de DbContext
var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = ConvertDatabaseUrlToConnectionString(rawUrl!);

builder.Services.AddDbContext<BaseHormigonContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CalculoBasesAIE API v1");
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Migración automática
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BaseHormigonContext>();
    db.Database.Migrate();
}

app.Run();
