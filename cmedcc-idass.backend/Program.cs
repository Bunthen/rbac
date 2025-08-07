using cmedcc_idass.backend.Config;
using Microsoft.EntityFrameworkCore;
using cmedcc_idass.backend.Services;
using cmedcc_idass.backend.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Register serivce
builder.Services.AddScoped<IUserService,UserService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
