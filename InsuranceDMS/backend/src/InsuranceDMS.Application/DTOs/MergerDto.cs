using InsuranceDMS.Domain.Enums;

namespace InsuranceDMS.Application.DTOs;

public class MergerDto
{
    public int Id { get; set; }
    public int SurvivingAgencyId { get; set; }
    public string? SurvivingAgencyName { get; set; }
    public MergerStatus Status { get; set; }
    public MergerType MergerType { get; set; }
    public string? InitiatedBy { get; set; }
    public DateTime InitiatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? ExecutedBy { get; set; }
    public string? Notes { get; set; }
    public List<MergerParticipantDto> Participants { get; set; } = new();
}

public class MergerParticipantDto
{
    public int Id { get; set; }
    public int AbsorbedAgencyId { get; set; }
    public string? AbsorbedAgencyName { get; set; }
    public int? AbsorbedLocationId { get; set; }
    public string? AbsorbedLocationName { get; set; }
    public int PersonnelTransferred { get; set; }
}

public class CreateMergerDto
{
    public int SurvivingAgencyId { get; set; }
    public List<int> AbsorbedAgencyIds { get; set; } = new();
    public string? Notes { get; set; }
    public MergerType MergerType { get; set; } = MergerType.Agency;
}

public class CreateLocationMergerDto
{
    public int AcquiringAgencyId { get; set; }
    public int AbsorbedLocationId { get; set; }
    public string? Notes { get; set; }
}

public class MergerPreviewDto
{
    public int MergerId { get; set; }
    public int SurvivingAgencyId { get; set; }
    public string? SurvivingAgencyName { get; set; }
    public List<AbsorbedAgencyPreviewDto> AbsorbedAgencies { get; set; } = new();
    public List<AbsorbedLocationPreviewDto> AbsorbedLocations { get; set; } = new();
    public List<string> Conflicts { get; set; } = new();
    public int TotalPersonnelToTransfer { get; set; }
}

public class AbsorbedAgencyPreviewDto
{
    public int AgencyId { get; set; }
    public string AgencyName { get; set; } = null!;
    public int PersonnelCount { get; set; }
    public int ProducerCount { get; set; }
    public int LicenseCount { get; set; }
    public int AppointmentCount { get; set; }
    public List<string> DuplicateNPNs { get; set; } = new();
}

public class EntityLineageDto
{
    public int Id { get; set; }
    public int MergerId { get; set; }
    public string EntityType { get; set; } = null!;
    public int SourceEntityId { get; set; }
    public int? TargetEntityId { get; set; }
    public int SourceAgencyId { get; set; }
    public string? SourceAgencyName { get; set; }
    public int TargetAgencyId { get; set; }
    public string? TargetAgencyName { get; set; }
    public string Action { get; set; } = null!;
    public DateTime RecordedAt { get; set; }
}

public class CarrierDto
{
    public int Id { get; set; }
    public string CarrierName { get; set; } = null!;
    public string? NAIC { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCarrierDto
{
    public string CarrierName { get; set; } = null!;
    public string? NAIC { get; set; }
}
