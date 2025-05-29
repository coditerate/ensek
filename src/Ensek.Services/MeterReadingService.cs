using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Repository.Models;
using Ensek.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Ensek.Services.Mappers;

namespace Ensek.Services;

public class MeterReadingService(EnsekContext dbContext) : IMeterReadingService
{
    public async Task<(bool Success, string? ErrorMessage, UploadResult? Result)> PostAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        try
        {
            // Validate The File Uploaded
            var fileValidation = await ValidateFile(file);
            if (!fileValidation.Success) return (fileValidation.Success, fileValidation.ErrorMessage, null);

            // Read And Validate File Data
            var (totalRecords, validRecords, invalidRecords) = await ValidateFileData(file);

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
    
    private async Task<(int TotalRecords, List<MeterReadingDTO> ValidRecords, List<string> InvalidRecords)> ValidateFileData(IFormFile? file)
    {
        var totalRecords = 0;
        var invalidRecords = new List<string>();
        var validRecords = new List<MeterReadingDTO>();
        var duplicateCheck = new HashSet<(int AccountId, DateTime ReadDate)>();

        // Load All AccountIds From The Account DB Table
        var validAccountIds = await dbContext.Set<Account>()
            .Select(a => a.AccountId)
            .ToHashSetAsync();

        // Load All Meter Readings From The MeterReading DB Table
        var existingKeys = await dbContext.Set<MeterReading>()
            .Select(r => new { r.AccountId, r.MeterReadingDateTime })
            .ToHashSetAsync();

        // Get Latest Meter Readings Per Account For Each AccountId
        var latestReadingsByAccount = existingKeys.GroupBy(r => r.AccountId)
            .Select(g => new { AccountId = g.Key, LatestDate = g.Max(x => x.MeterReadingDateTime) })
            .ToDictionary(g => g.AccountId, g => g.LatestDate);

        // Read CSV Records One At A Time

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            IgnoreBlankLines = true,
            MissingFieldFound = null,       // Prevent exception on missing fields
            BadDataFound = null,            // Prevent exception on bad data
            HeaderValidated = null,         // Ignore header validation errors
            TrimOptions = TrimOptions.Trim, // Remove whitespace
            HasHeaderRecord = true
        };

        using var streamReader = new StreamReader(file!.OpenReadStream());
        using var csv = new CsvReader(streamReader, config);

        await foreach (var record in csv.GetRecordsAsync<MeterReadingDTO>())
        {
            totalRecords++;
            var key = (record.AccountId, record.MeterReadingDateTime);

            // Validate AccountId Exists
            if (!validAccountIds.Contains(record.AccountId))
            {
                invalidRecords.Add($"Invalid AccountId: {record.AccountId}");
                continue;
            }

            // Validate MeterReadingValue Between 0 And 99999
            if (record.MeterReadValue < 0 || record.MeterReadValue > 99999)
            {
                invalidRecords.Add($"Invalid MeterReadValue for AccountId {record.AccountId}: Must be 00000–99999.");
                continue;
            }

            // Validate For Duplicate Value In The Current CSV
            if (!duplicateCheck.Add(key))
            {
                invalidRecords.Add($"Duplicate in CSV for AccountId {record.AccountId} at {record.MeterReadingDateTime}");
                continue;
            }

            // Validate For Duplicate Value In The DB
            if (existingKeys.Contains(new { record.AccountId, record.MeterReadingDateTime}))
            {
                invalidRecords.Add($"Duplicate in DB for AccountId {record.AccountId} at {record.MeterReadingDateTime}");
                continue;
            }

            // Validate For A Newer Reading Than An Existing One, If Any
            if (latestReadingsByAccount.TryGetValue(record.AccountId, out var latestDate) &&
                record.MeterReadingDateTime <= latestDate)
            {
                invalidRecords.Add($"Reading is older than latest for AccountId {record.AccountId}.");
                continue;
            }

            // Passed All Validation
            validRecords.Add(record);
        }

        return (totalRecords, validRecords, invalidRecords);
    }


    private async Task<(bool Success, string? ErrorMessage)> ValidateFile(IFormFile? file)
    {
        if (file == null || file.Length == 0) return (false, "No file uploaded.");
        if (Path.GetExtension(file.FileName) != ".csv") return (false, "Only .csv files are allowed.");
        if (file.ContentType != "text/csv" && file.ContentType != "application/vnd.ms-excel") return (false, "Invalid content type.");

        using var reader = new StreamReader(file.OpenReadStream());
        var headerLine = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(headerLine) || !headerLine.Contains(',')) return (false, "The uploaded file doesn't appear to be a valid CSV.");

        return (true, null);
    }
}
