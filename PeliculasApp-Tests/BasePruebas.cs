

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Utilidades;

namespace PeliculasApp_Tests;

public class BasePruebas
{
    protected ApplicationDbContext ConstruirContext(string nombreDB)
    {
        DbContextOptions<ApplicationDbContext>? opciones = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(nombreDB).Options;
        
        ApplicationDbContext? dbContext = new ApplicationDbContext(opciones);

        return dbContext;
    }

    protected IMapper ConfigurarAutoMapper()
    {
        MapperConfiguration? config = new MapperConfiguration(opt =>
        {
            GeometryFactory? geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            opt.AddProfile(new AutoMapperProfiles(geometryFactory));
        });
        
        return config.CreateMapper();
    }
}