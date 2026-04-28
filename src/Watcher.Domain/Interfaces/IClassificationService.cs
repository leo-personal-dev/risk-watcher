using Watcher.Domain.Entities;

namespace Watcher.Domain.Interfaces;

public interface IClassificationService
{
    Task<ClassificationResult> ClassifyAsync(Customer customer);
}