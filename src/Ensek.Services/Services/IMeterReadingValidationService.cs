using Ensek.Services.Models;
using Microsoft.AspNetCore.Http;

namespace Ensek.Services.Services;

public interface IMeterReadingValidationService
{
    Task<(int TotalRecords, List<MeterReadingDTO> ValidRecords, List<string> InvalidRecords)> ValidateFileData(IFormFile? file);
    Task<(bool Success, string? ErrorMessage)> ValidateFile(IFormFile? file);
}