namespace Ensek.Services.Models;

public partial class MeterReadingDTO
{
    public int AccountId { get; set; }

    [CsvHelper.Configuration.Attributes.Format("dd/MM/yyyy HH:mm")]
    public DateTime MeterReadingDateTime { get; set; }

    public int MeterReadValue { get; set; }
}
