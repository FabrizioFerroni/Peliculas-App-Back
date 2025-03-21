namespace PeliculasApp_Back.Servicios.Interfaces;

public interface IAlmacenadorArchivos
{
    Task<string> Almacenar(string contenedor, IFormFile archivo);
    Task Borrar(string? url, string contenedor);

    async Task<string> Editar(string? url, string contenedor, IFormFile archivo)
    {
        await Borrar(url, contenedor);
        return await Almacenar(contenedor, archivo);
    }
}