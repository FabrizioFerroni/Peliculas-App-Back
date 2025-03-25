using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Servicios;
using PeliculasApp_Back.Servicios.Interfaces;
using PeliculasApp_Back.Utilidades;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;

services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

services.AddHttpContextAccessor();
services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

// services.AddAutoMapper(typeof(Program));

services.AddSingleton(prov =>
    new MapperConfiguration(conf =>
    {
        GeometryFactory? geometryFactory = prov.GetRequiredService<GeometryFactory>();
        conf.AddProfile(new AutoMapperProfiles(geometryFactory));
    }).CreateMapper()
);

services.AddIdentityCore<IdentityUser<Guid>>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

services.AddScoped<UserManager<IdentityUser<Guid>>>();
services.AddScoped<SignInManager<IdentityUser<Guid>>>();

services.AddAuthentication().AddJwtBearer(opt =>
{
    opt.MapInboundClaims = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", policy => policy.RequireClaim("esadmin"));
});

string connectionString = builder.Configuration.GetConnectionString("SQLServerDB")!;

services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(connectionString, ss => ss.UseNetTopologySuite());
});

services.AddOutputCache(opt =>
{
    opt.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

//configure cors for app
string[] origins = builder.Configuration.GetValue<string>("Origins")!.Split(",");

services.AddCors(opt =>
{
    opt.AddDefaultPolicy(optCors =>
    {
        optCors.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("cantidad-total-registros");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();

app.Run();