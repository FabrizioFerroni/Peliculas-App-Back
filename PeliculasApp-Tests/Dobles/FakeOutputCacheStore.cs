using Microsoft.AspNetCore.OutputCaching;

public class FakeOutputCacheStore : IOutputCacheStore
{
    public ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        // Simula que se evicta el caché
        return ValueTask.CompletedTask;
    }

    public ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        // Simula que no hay caché para esta clave
        return new ValueTask<byte[]?>(default(byte[]?));
    }

    public ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        // Simula que se guarda algo en caché
        return ValueTask.CompletedTask;
    }
}