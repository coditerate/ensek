using Ensek.Repository.Models;
using Ensek.Services.Models;
using Microsoft.AspNetCore.Http;
using Ensek.Services.Mappers;

namespace Ensek.Services.Services;

public class MeterReadingService(IMeterReadingValidationService validationService, EnsekContext dbContext) : IMeterReadingService
{
    public async Task<(bool Success, string? ErrorMessage, UploadResult? Result)> PostAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        try
        {
            // Validate The File Uploaded
            var fileValidation = await validationService.ValidateFile(file);
            if (!fileValidation.Success) return (fileValidation.Success, fileValidation.ErrorMessage, null);

            // Read And Validate File Data
            var (totalRecords, validRecords, invalidRecords) = await validationService.ValidateFileData(file);

            // Store File Data In Database
            if (validRecords.Any())
            {
                dbContext.AddRange(validRecords.Select(record => record.MapToMeterReading()));
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return (true, null, new UploadResult
            {
                TotalRecords = totalRecords,
                SuccessfulRecords = validRecords.Count,
                FailedRecords = invalidRecords.Count
            });
        }
        catch (Exception ex)
        {
            // Log ex and ex.Message somewhere
            return (false, "Something went wrong.", null);
        }
    }
}
