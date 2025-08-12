using cmedcc_idass.backend.Config;
using Microsoft.EntityFrameworkCore;
using cmedcc_idass.backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using cmedcc_idass.backend.Middleware;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(connectionString));
var connectionStringAuth =  builder.Configuration.GetConnectionString("AuthConnectionDev");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionStringAuth));
// Add Identity service

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();
// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.Zero
    };
});
//Register serivce
//builder.Services.AddScoped<IUserService,UserService>();  //Register User Service
builder.Services.AddScoped<IAuthService, AuthService>(); //Register Auth Service
builder.Services.AddScoped<IAdminService, AdminService>(); //Register Admin Service

builder.Services.AddControllers();
////ssss
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("*") // <-- Your Blazor WASM URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
    app.UseDeveloperExceptionPage();
}
//Middleware registering
//app.UseMiddleware<ApiKeyMiddleware>();
app.UseCors("AllowBlazorClient");  //this to diable cors
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
