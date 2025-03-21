using MongoDB.Bson;
using PeliculasApp_Back.Servicios.Interfaces;

namespace PeliculasApp_Back.Servicios;

public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<string> Almacenar(string contenedor, IFormFile archivo)
    {
        string extension = Path.GetExtension(archivo.FileName);
        string nombreArchivo = $"{ObjectId.GenerateNewId()}{extension}";
        string folder = Path.Combine(_env.WebRootPath, contenedor);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        string ruta = Path.Combine(folder, nombreArchivo);

        using (var ms = new MemoryStream())
        {
            await archivo.CopyToAsync(ms);
            Byte[] contenido = ms.ToArray();
            await File.WriteAllBytesAsync(ruta, contenido);
        }
        
        HttpRequest? request = _httpContextAccessor.HttpContext!.Request!;

        string url = $"{request.Scheme}://{request.Host}";
        string urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace('\\', '/');
        
        return urlArchivo;
    }

    public Task Borrar(string? url, string contenedor)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Task.CompletedTask;
        }

        string nombreArchivo = Path.GetFileName(url);
        string directorioArchivo = Path.Combine(_env.WebRootPath, contenedor, nombreArchivo);

        if (File.Exists(directorioArchivo))
        {
            File.Delete(directorioArchivo);
        }
        
        return Task.CompletedTask;
    }
}