namespace CustomerService.Infrastructure.FileStorage;

public static class FileValidation
{
    private static readonly string[] ImageExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private static readonly string[] DocumentExtensions =
    {
        ".pdf"
    };

    public static bool IsValidProfileImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > 5 * 1024 * 1024)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return ImageExtensions.Contains(extension);
    }

    public static bool IsValidDocument(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > 10 * 1024 * 1024)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return DocumentExtensions.Contains(extension);
    }
}