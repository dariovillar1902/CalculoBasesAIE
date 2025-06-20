using CalculoBasesAIE.Models;
using CalculoBasesAIE.Repositories.BaseHormigonRepository;
using CalculoBasesAIE.Services.BaseHormigonIOService;
using CalculoBasesAIE.Services.BaseHormigonService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IBaseHormigonRepository, BaseHormigonRepository>();
builder.Services.AddScoped<IBaseHormigonService, BaseHormigonService>();
builder.Services.AddScoped<IBaseHormigonIOService, BaseHormigonIOService>();
builder.Services.AddScoped<BaseHormigonService>();
builder.Services.AddScoped<BaseHormigonIOService>();
builder.Services.AddDbContext<BaseHormigonContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
