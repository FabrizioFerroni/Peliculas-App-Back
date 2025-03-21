using PeliculasApp_Back.Servicios.Interfaces;

namespace PeliculasApp_Back.Servicios;

public class AlmacenadorArchivosAzure: IAlmacenadorArchivos
{
    private string _configuration;
    public AlmacenadorArchivosAzure(IConfiguration configuration)
    {
        _configuration = configuration.GetConnectionString("AzureStorageConnection")!;
    }
    
    public async Task<string> Almacenar(string contenedor, IFormFile archivo)
    {
        throw new NotImplementedException();
    }

    public async Task Borrar(string? url, string contenedor)
    {
        throw new NotImplementedException();
    }
}