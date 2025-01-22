using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Repositorio;

public class RepositorioEnMemoria
{
    private List<Genero> generos = new List<Genero>();

    public RepositorioEnMemoria()
    {
        generos = new List<Genero>()
        {
            new Genero()
            {
                Id = Guid.NewGuid(),
                Nombre = "Comedia",
            },
            new Genero(){
                Id = Guid.NewGuid(),
                Nombre = "Accion",
            },
            new Genero()
            {
                Id = new Guid("2368c8e4-5d0b-4589-9230-015d16330850"),
                Nombre = "Romance",
            }
        };
    }

    public List<Genero> ObtenerTodosLosGeneros()
    {
        return generos;
    }

    public async Task<Genero?> ObtenerGeneroPorId(Guid id)
    {
        await Task.Delay(TimeSpan.FromSeconds(3)); //Esto es por la db en memoria
        Genero? genero =  generos.FirstOrDefault(x => x.Id.Equals(id));
        return genero;
    }

    public void AgregarGenero(Genero genero)
    {
        generos.Add(genero);
    }
}