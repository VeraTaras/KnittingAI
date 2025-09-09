using System.IO;
using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Контракт запуска AI‑модели.
/// Инфраструктурные реализации (HTTP/Docker и т.п.) должны его реализовать.
/// </summary>
public interface IModelRunner
{
    /// <summary>
    /// Запускает модель, передавая поток изображения, и возвращает данные результата.
    /// </summary>
    ModelOutputData RunModel(Stream image);
}
