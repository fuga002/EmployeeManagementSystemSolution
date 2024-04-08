using EMS.APILibrary.Data;
using EMS.APILibrary.Helpers;
using EMS.APILibrary.Repositories.Contracts;
using EMS.APILibrary.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")??throw new Exception("Connection string not found"));
});
builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
builder.Services.AddScoped<IUserAccount, UserAccountRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWebApp",
        config => 
            config.WithOrigins("https://localhost:7016","http://localhost:5158")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorWebApp");
app.UseAuthorization();

app.MapControllers();

app.Run();
