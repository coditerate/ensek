using Ensek.Services.Models;
using Microsoft.AspNetCore.Http;

namespace Ensek.Services.Services;

public interface IMeterReadingService
{
    Task<(bool Success, string? ErrorMessage, UploadResult? Result)> PostAsync(IFormFile? file, CancellationToken cancellationToken);
}
