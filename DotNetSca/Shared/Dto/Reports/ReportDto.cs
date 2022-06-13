namespace Shared.Dto.Reports;

public class ReportDto
{
    public int Id { get; set; }
    
    public string Initiator { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public bool IsFilledByServer { get; set; }
}