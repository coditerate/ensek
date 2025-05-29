namespace Ensek.Services.Models;

public class UploadResult
{
    public int TotalRecords { get; set; }

    public int SuccessfulRecords { get; set; }

    public int FailedRecords { get; set; }
}
