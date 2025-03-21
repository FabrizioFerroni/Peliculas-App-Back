using AutoMapper;
using NetTopologySuite.Geometries;
using PeliculasApp_Back.Dtos;
using PeliculasApp_Back.Entidades;

namespace PeliculasApp_Back.Utilidades;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles(GeometryFactory geometryFactory)
    {
        ConfigurarMapeoGeneros();
        ConfigurarMapeActores();
        ConfigurarMapeoCines(geometryFactory);
        ConfigurarMapeoPeliculas();
    }

    private void ConfigurarMapeoGeneros()
    {
        CreateMap<GeneroCreacionDto, Genero>();
        CreateMap<Genero, GeneroDto>();
    }
    
    private void ConfigurarMapeActores()
    {
        CreateMap<ActorCreacionDto, Actor>()
            .ForMember(a => a.Foto, opt => opt.Ignore());
        CreateMap<Actor, ActorDto>();
        CreateMap<Actor, PeliculaActorDto>();
    }

    private void ConfigurarMapeoCines(GeometryFactory geometryFactory)
    {
        CreateMap<Cine, CineDto>()
            .ForMember(x => x.Latitud, c => c.MapFrom(p => p.Ubicacion.Y))
            .ForMember(x => x.Longitud, c => c.MapFrom(p => p.Ubicacion.X));

        CreateMap<CineCreacionDto, Cine>()
            .ForMember(x => x.Ubicacion, c => c.MapFrom(p => geometryFactory.CreatePoint(new Coordinate(p.Latitud, p.Longitud))));
    }

    private void ConfigurarMapeoPeliculas()
    {
        CreateMap<PeliculaCreacionDto, Pelicula>()
            .ForMember(p => p.Poster, opt => opt.Ignore())
            .ForMember(p => p.PeliculasGeneros,
                dto => dto.MapFrom(g => g.GenerosIds!.Select(id => new PeliculaGenero() { GeneroId = id })))
            .ForMember(p => p.PeliculasCines,
                dto => dto.MapFrom(g => g.CinesIds!.Select(id => new PeliculaCine() { CineId = id })))
            .ForMember(p => p.PeliculasActores,
                dto => dto.MapFrom(g => g.Actores!.Select(actor => new PeliculaActor()
                    { ActorId = actor.Id, Personaje = actor.Personaje })));
        
        
    
        CreateMap<Pelicula, PeliculaDto>();
       
        CreateMap<Pelicula, PeliculaDetallesDto>()
            .ForMember(p => p.Generos, ent => ent.MapFrom(p => p.PeliculasGeneros))
            .ForMember(p => p.Cines, ent => ent.MapFrom(p => p.PeliculasCines))
            .ForMember(p => p.Actores, ent => ent.MapFrom(p => p.PeliculasActores.OrderBy(oa => oa.Orden)));
    
        CreateMap<PeliculaGenero, GeneroDto>()
            .ForMember(g => g.Id, pg => pg.MapFrom(p => p.GeneroId))
            .ForMember(g => g.Nombre, pg => pg.MapFrom(p => p.Genero.Nombre))
            .ForMember(g => g.Slug, pg => pg.MapFrom(p => p.Genero.Slug));
        
        CreateMap<PeliculaCine, CineDto>()
            .ForMember(c => c.Id, pc => pc.MapFrom(p => p.CineId))
            .ForMember(c => c.Nombre, pc => pc.MapFrom(p => p.Cine.Nombre))
            .ForMember(c => c.Latitud, pc => pc.MapFrom(p => p.Cine.Ubicacion.Y))
            .ForMember(c => c.Longitud, pc => pc.MapFrom(p => p.Cine.Ubicacion.X));
    
        CreateMap<PeliculaActor, PeliculaActorDto>()
            .ForMember(a => a.Id, pg => pg.MapFrom(p => p.ActorId))
            .ForMember(a => a.Nombre, pg => pg.MapFrom(p => p.Actor.Nombre))
            .ForMember(a => a.Foto, pg => pg.MapFrom(p => p.Actor.Foto));
        
        // CreateMap<Pelicula, PeliculaActorDto>();
    }
    
     
    
    
}