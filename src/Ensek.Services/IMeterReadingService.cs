using Ensek.Services.Models;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Ensek.Services;

public interface IMeterReadingService
{
    Task<(bool Success, string? ErrorMessage, UploadResult? Result)> PostAsync(IFormFile? file, CancellationToken cancellationToken);
}
