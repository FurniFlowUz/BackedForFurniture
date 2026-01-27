using Microsoft.AspNetCore.Http;

namespace FurniFlowUz.Infrastructure.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to the storage
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="entityType">The entity type (e.g., "drawings", "contracts")</param>
    /// <returns>The file name</returns>
    Task<string> UploadFileAsync(IFormFile file, string entityType);

    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="fileName">The file name to delete</param>
    /// <param name="entityType">The entity type</param>
    Task DeleteFileAsync(string fileName, string entityType);

    /// <summary>
    /// Gets the full file path
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <param name="entityType">The entity type</param>
    /// <returns>The full file path</returns>
    string GetFilePath(string fileName, string entityType);

    /// <summary>
    /// Gets the file URL for serving
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <param name="entityType">The entity type</param>
    /// <returns>The relative URL path</returns>
    string GetFileUrl(string fileName, string entityType);

    /// <summary>
    /// Checks if a file exists
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <param name="entityType">The entity type</param>
    /// <returns>True if the file exists, false otherwise</returns>
    bool FileExists(string fileName, string entityType);
}

public class FileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private const string UploadFolder = "uploads";
    private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".dwg", ".dxf", ".doc", ".docx", ".xls", ".xlsx" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public FileStorageService(string rootPath)
    {
        _rootPath = rootPath;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string entityType)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null.", nameof(file));
        }

        // Validate file size
        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds the maximum allowed size of {MaxFileSize / 1024 / 1024} MB.");
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
        }

        // Generate unique file name
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        // Create directory if it doesn't exist
        var uploadPath = Path.Combine(_rootPath, UploadFolder, entityType);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        // Save file
        var filePath = Path.Combine(uploadPath, uniqueFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }

    public Task DeleteFileAsync(string fileName, string entityType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is empty or null.", nameof(fileName));
        }

        var filePath = GetFilePath(fileName, entityType);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    public string GetFilePath(string fileName, string entityType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is empty or null.", nameof(fileName));
        }

        return Path.Combine(_rootPath, UploadFolder, entityType, fileName);
    }

    public string GetFileUrl(string fileName, string entityType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is empty or null.", nameof(fileName));
        }

        return $"/{UploadFolder}/{entityType}/{fileName}";
    }

    public bool FileExists(string fileName, string entityType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        var filePath = GetFilePath(fileName, entityType);
        return File.Exists(filePath);
    }
}
