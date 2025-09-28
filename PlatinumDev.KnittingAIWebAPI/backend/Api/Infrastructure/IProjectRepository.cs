using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Kontrakt repozytorium projektów.
/// Możliwe implementacje: InMemory, EF Core, plikowe itp.
/// </summary>
public interface IProjectRepository
{
    void Save(KnittingProject project);
    KnittingProject? Load(Guid id);
    IEnumerable<KnittingProject> GetAll();
}
