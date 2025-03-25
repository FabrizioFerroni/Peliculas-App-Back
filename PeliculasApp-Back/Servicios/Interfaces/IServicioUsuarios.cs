namespace PeliculasApp_Back.Servicios.Interfaces;

public interface IServicioUsuarios
{
    Task<Guid> ObtenerUsuario();
}