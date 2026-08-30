namespace CustomerService.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "customers", folder);
        Directory.CreateDirectory(uploadsFolder);
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);
        _logger.LogInformation("File saved: {FileName}", fileName);
        return Path.Combine( "uploads", "customers", folder, fileName).Replace("\\", "/");
    }

    public Task DeleteAsync(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.CompletedTask;
        var physicalPath = Path.Combine( _environment.WebRootPath, filePath);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
            _logger.LogInformation("File deleted: {FilePath}", filePath);
        }

        return Task.CompletedTask;
    }

    public bool Exists(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return false;
        var physicalPath = Path.Combine( _environment.WebRootPath, filePath);
        return File.Exists(physicalPath);
    }
}