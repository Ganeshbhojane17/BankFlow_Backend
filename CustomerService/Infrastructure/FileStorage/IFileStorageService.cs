namespace CustomerService.Infrastructure.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
        Task DeleteAsync(string? filePath);
        bool Exists(string? filePath);
    }
}
