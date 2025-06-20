using CalculoBasesAIE.Models;
using CalculoBasesAIE.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173") // Allow frontend
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddDbContext<BaseHormigonContext>(opt =>
    opt.UseInMemoryDatabase("BaseHormigon"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend"); // Use the policy

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
