using System.Collections.Concurrent;
using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Kontrakt repozytorium projektów. 
/// Możliwe implementacje: InMemory, EF Core, plikowe itp.
/// </summary>
public interface IProjectRepository
{
    public void Save(KnittingProject project) 
        => _db[project.Id] = project;

    public KnittingProject? Load(Guid id) 
        => _db.TryGetValue(id, out var p) ? p : null;

    public IEnumerable<KnittingProject> GetAll() 
        => _db.Values.OrderByDescending(p => p.CreatedUtc);
}
