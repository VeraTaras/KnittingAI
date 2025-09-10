using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Kontrakt repozytorium projektów. 
/// Możliwe implementacje: InMemory, EF Core, plikowe itp.
/// </summary>
public interface IProjectRepository
{
    void Save(PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject project);
    PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject? Load(Guid id);
    IEnumerable<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> GetAll();
}
