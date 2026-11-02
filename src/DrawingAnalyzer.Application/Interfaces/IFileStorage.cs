namespace DrawingAnalyzer.Application.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(
        Stream content,
        string storageFileName,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken);
}