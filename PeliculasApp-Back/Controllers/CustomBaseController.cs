using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApp_Back.Annotations;
using PeliculasApp_Back.Data;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Dtos.Response;
using PeliculasApp_Back.Entidades;
using PeliculasApp_Back.Utilidades;

namespace PeliculasApp_Back.Controllers;

public class CustomBaseController: ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public CustomBaseController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    protected async Task<List<TDto>> GetAllAsync<TEntidad, TDto>(Expression<Func<TEntidad, object>> ordenarPor)
        where TEntidad : class
    {
        List<TDto> data = await context.Set<TEntidad>()
            .OrderBy(ordenarPor)
            .ProjectTo<TDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        
        return data;
    }

    protected async Task<Pageable<List<TDto>>> Get<TEntidad, TDto>(PaginationDto pagination, Expression<Func<TEntidad, object>> buscarPor) where TEntidad: class
    {
        IQueryable<TEntidad> queryable = context.Set<TEntidad>().AsQueryable();
        
        if (!string.IsNullOrEmpty(pagination.Search))
        {
            ParameterExpression param = Expression.Parameter(typeof(TEntidad), "e");

            MemberExpression propertyAccess = Expression.Property(param, ((MemberExpression)buscarPor.Body).Member.Name);

            ConstantExpression searchValue = Expression.Constant(pagination.Search);
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            MethodCallExpression containsExpression = Expression.Call(propertyAccess, containsMethod, searchValue);

            Expression<Func<TEntidad, bool>> lambda = Expression.Lambda<Func<TEntidad, bool>>(containsExpression, param);

            queryable = queryable.Where(lambda);
        }
            
        int totalElements = await queryable.CountAsync();
        
        await HttpContext.InsertarParametrosPaginacionEnCabezera(queryable);
        
        queryable = pagination.Ascending
            ? queryable.OrderBy(e => EF.Property<object>(e, pagination.Ordenar))
            : queryable.OrderByDescending(e => EF.Property<object>(e, pagination.Ordenar));
        
        List<TDto> resDto = await queryable
            .Paginar(pagination)
            .ProjectTo<TDto>(mapper.ConfigurationProvider).ToListAsync();

        Pageable<List<TDto>> response = PageableResponse.CreatePageableResponse(resDto, pagination.Pagina, pagination.RegistrosPorPagina, totalElements);

        return response;
    }

    protected async Task<ActionResult<TDto>> GetById<TEntidad, TDto>(Guid id, string mensaje) where TEntidad : class, IId where TDto : IId
    {
        TDto? entity = await context.Set<TEntidad>().ProjectTo<TDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(e => e.Id.Equals(id));

        if (entity == null)
        {
            return NotFound(new { Mensaje = mensaje });
        }
        
        return Ok(entity);
        
    }
    
    protected async Task<ActionResult<TDto>> GetBySlug<TEntidad, TDto>(string slug, string mensaje) where TEntidad : class, IId where TDto : IId
    {
        TDto? entity = await context.Set<TEntidad>().ProjectTo<TDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(e => e.Slug!.Equals(slug));

        if (entity == null)
        {
            return NotFound(new { Mensaje = mensaje });
        }
        
        return Ok(entity);
        
    }
}