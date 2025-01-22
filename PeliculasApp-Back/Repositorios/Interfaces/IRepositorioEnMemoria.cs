using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Repositorios.Interfaces;

public interface IRepositorioEnMemoria
{
    List<Genero> ObtenerTodosLosGeneros();
    Task<Genero?> ObtenerGeneroPorId(Guid id);
    void AgregarGenero(Genero genero);
    bool Existe(string nombre);
}