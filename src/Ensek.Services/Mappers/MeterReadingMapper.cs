using Ensek.Repository.Models;
using Ensek.Services.Models;

namespace Ensek.Services.Mappers;

public static class MeterReadingMapper
{
    public static MeterReadingDTO MapToMeterReadingDTO(this MeterReading meterReading)
    {
        return new MeterReadingDTO
        {
            AccountId = meterReading.AccountId,
            MeterReadingDateTime = meterReading.MeterReadingDateTime,
            MeterReadValue = meterReading.MeterReadValue
        };
    }

    public static MeterReading MapToMeterReading(this MeterReadingDTO meterReadingDto)
    {
        return new MeterReading
        {
            AccountId = meterReadingDto.AccountId,
            MeterReadingDateTime = meterReadingDto.MeterReadingDateTime,
            MeterReadValue = meterReadingDto.MeterReadValue
        };
    }
}
