using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Ensek.Services.Helpers;

public static class CsvHelper
{
    public static (StreamReader, CsvConfiguration) GetStreamReader(this IFormFile? file)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            IgnoreBlankLines = true,
            MissingFieldFound = null,       // Prevent exception on missing fields
            BadDataFound = null,            // Prevent exception on bad data
            HeaderValidated = null,         // Ignore header validation errors
            TrimOptions = TrimOptions.Trim, // Remove whitespace
            HasHeaderRecord = true
        };

        return (new StreamReader(file!.OpenReadStream()), config);
    }
}
