using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

// services.AddAutoMapper(typeof(Program));

services.AddSingleton(prov =>
    new MapperConfiguration(conf =>
    {
        GeometryFactory? geometryFactory = prov.GetRequiredService<GeometryFactory>();
        conf.AddProfile(new AutoMapperProfiles(geometryFactory));
    }).CreateMapper()
);

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