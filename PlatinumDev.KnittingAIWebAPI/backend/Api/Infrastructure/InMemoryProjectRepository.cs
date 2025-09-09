using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Контракт хранилища проектов. Можно иметь реализации: InMemory, EF Core, файл и т.п.
/// </summary>
public interface IProjectRepository
{
    void Save(PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject project);
    PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject? Load(Guid id);
    IEnumerable<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> GetAll();
}
