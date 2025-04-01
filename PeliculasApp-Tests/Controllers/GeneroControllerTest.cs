using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using NSubstitute;
using PeliculasApp_Back.Controllers;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Tests.Controllers;

[TestClass]
public sealed class GeneroControllerTest: BasePruebas
{
    private const string CacheKey = "Generos";
    
    [TestMethod]
    public async Task Get_ReturnsAllGenders()
    {
        // Preparación
        string randomNameDb = ObjectId.GenerateNewId().ToString();
        
        ApplicationDbContext? context = ConstruirContext(randomNameDb);

        IMapper? mapper = ConfigurarAutoMapper();

        FakeOutputCacheStore? outputCacheStoreFake = new FakeOutputCacheStore();
        
        context.Generos.Add(new Genero()
        {
            Nombre = "Comedia",
            Slug = "comedia"
        });
        context.Generos.Add(new Genero()
        {
            Nombre = "Acción",
            Slug = "accion"
        });
        
        await context.Database.EnsureCreatedAsync();
        await context.SaveChangesAsync();
        
        ApplicationDbContext? context2 = ConstruirContext(randomNameDb);
        
        GenerosController? controller = new GenerosController(outputCacheStoreFake, context2, mapper);
        
        
        //Prueba
        ActionResult<List<GeneroDto>>? respuesta = await controller.GetAllGeneros();
        
        //Verificacion
        Assert.IsInstanceOfType(respuesta.Result, typeof(OkObjectResult));
    
        // Verificación de que la lista tiene 2 elementos
        List<GeneroDto>? generos = ((OkObjectResult)respuesta.Result).Value as List<GeneroDto>;
        
        Assert.IsNotNull(generos);
        Assert.AreEqual(2, generos.Count);
    }

    [TestMethod]
    public async Task Get_MustReturn_404_WhenGenderWithIdDoesNotExist()
    {
        // Preparación
        string randomNameDb = ObjectId.GenerateNewId().ToString();
        
        ApplicationDbContext? context = ConstruirContext(randomNameDb);

        IMapper? mapper = ConfigurarAutoMapper();

        FakeOutputCacheStore? outputCacheStoreFake = new FakeOutputCacheStore();
        
        GenerosController? controller = new GenerosController(outputCacheStoreFake, context, mapper);
        
        Guid id = Guid.NewGuid();
        
        // Prueba 
        ActionResult<GeneroDto>? respuesta = await controller.GetGenero(id);
        
        ObjectResult? resultado = respuesta.Result as ObjectResult;

        Assert.IsNotNull(resultado);
        Assert.AreEqual(404, resultado?.StatusCode);
    }
    
    [TestMethod]
    public async Task Get_MustReturnCorrectGender_WhenGenderWithIdExists()
    {
        // Preparación
        string randomNameDb = ObjectId.GenerateNewId().ToString();
        
        ApplicationDbContext? context = ConstruirContext(randomNameDb);

        IMapper? mapper = ConfigurarAutoMapper();

        FakeOutputCacheStore? outputCacheStoreFake = new FakeOutputCacheStore();
        
        Genero? generoComedia = new Genero()
        {
            Nombre = "Comedia",
            Slug = "comedia"
        };
        Genero? generoAccion = new Genero()
        {
            Nombre = "Acción",
            Slug = "accion"
        };

        context.Generos.Add(generoComedia);
        context.Generos.Add(generoAccion);
        
        await context.Database.EnsureCreatedAsync();
        await context.SaveChangesAsync();
        
        ApplicationDbContext? context2 = ConstruirContext(randomNameDb);
        
        GenerosController? controller = new GenerosController(outputCacheStoreFake, context2, mapper);
        
        Guid id = generoComedia.Id;
        
        //Prueba
        ActionResult<GeneroDto>? respuesta = await controller.GetGenero(id);
        
        //Verificacion
        Assert.IsInstanceOfType(respuesta.Result, typeof(OkObjectResult));
    
        // Verificación de que la lista tiene 2 elementos
        GeneroDto? genero = ((OkObjectResult)respuesta.Result).Value as GeneroDto;
        
        Assert.IsNotNull(genero);
        Assert.AreEqual(id, genero.Id);
        Assert.AreEqual(generoComedia.Nombre, genero.Nombre);
    }

    [TestMethod]
    public async Task Post_MustCreateGender_WhenWeSendGender()
    {
        // Preparación
        string randomNameDb = ObjectId.GenerateNewId().ToString();
        
        ApplicationDbContext? context = ConstruirContext(randomNameDb);

        IMapper? mapper = ConfigurarAutoMapper();
        
        FakeOutputCacheStore? outputCacheStoreFake = new FakeOutputCacheStore();

        GeneroCreacionDto newGenero = new GeneroCreacionDto()
        {
            Nombre = "Comedia"
        };
        
        GenerosController? controller = new GenerosController(outputCacheStoreFake, context, mapper);
        
        // Prueba
        IActionResult? respuesta = await controller.Post(newGenero);
        
        // Verificación
        Assert.IsInstanceOfType(respuesta, typeof(CreatedAtActionResult));
    
        // Verificación de que la lista tiene 2 elementos
        object? genero = ((CreatedAtActionResult)respuesta).Value;
        
        Assert.IsNotNull(genero);
        
        ApplicationDbContext? context2 = ConstruirContext(randomNameDb);
        
        int cantidad = await context2.Generos.CountAsync();
        
        Assert.AreEqual(1, cantidad);
    }

    [TestMethod]
    public async Task Post_MustCallEvictByTagAsync_WhenWeSendGender()
    {
        // Preparación
        string randomNameDb = ObjectId.GenerateNewId().ToString();
        
        ApplicationDbContext? context = ConstruirContext(randomNameDb);

        IMapper? mapper = ConfigurarAutoMapper();

        var outputCacheStoreFake = Substitute.For<IOutputCacheStore>();

        GeneroCreacionDto newGenero = new GeneroCreacionDto()
        {
            Nombre = "Comedia"
        };
        
        GenerosController? controller = new GenerosController(outputCacheStoreFake, context, mapper);
        
        // Prueba
        IActionResult? respuesta = await controller.Post(newGenero);
        
        // Verificación
        Assert.IsInstanceOfType(respuesta, typeof(CreatedAtActionResult));
    
        // Verificación de que la lista tiene 2 elementos
        await outputCacheStoreFake.Received(1).EvictByTagAsync(CacheKey, default);
    }
}