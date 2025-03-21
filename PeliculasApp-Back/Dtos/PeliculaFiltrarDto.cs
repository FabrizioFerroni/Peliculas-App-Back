namespace PeliculasApp_Back.Dtos;

public class PeliculaFiltrarDto
{
    public int Pagina { get; set; } = 1;
    public int RecordsPorPagina { get; set; } = 10;

    internal PaginationDto Paginacion
    {
        get
        {
            return new PaginationDto
            {
                Pagina = Pagina,
                RegistrosPorPagina = RecordsPorPagina
            };
        }

    }
    
    public string? Titulo { get; set; }
    public string? AnioLanzamiento { get; set; }
    public string? Genero { get; set; }
    public bool EnCines { get; set; }
    public bool ProximoEstreno { get; set; }

}