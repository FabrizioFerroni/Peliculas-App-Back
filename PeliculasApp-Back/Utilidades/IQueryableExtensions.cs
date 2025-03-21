using PeliculasApp_Back.Dtos;

namespace PeliculasApp_Back.Utilidades;

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginationDto paginacioon)
    {
        return queryable
            .Skip((paginacioon.Pagina - 1) * paginacioon.RegistrosPorPagina)
            .Take(paginacioon.RegistrosPorPagina);
    }
}