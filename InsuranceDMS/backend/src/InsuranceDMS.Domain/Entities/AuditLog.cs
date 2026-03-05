namespace InsuranceDMS.Domain.Entities;

public class AuditLog
{
    public long AuditLogId { get; set; }
    public string TableName { get; set; } = null!;
    public int RecordId { get; set; }
    public string Action { get; set; } = null!;
    public string? ColumnName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public string? CorrelationId { get; set; }
}
