using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsuranceDMS.Infrastructure.Data;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MergersController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _db;

    public MergersController(IUnitOfWork uow, AppDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MergerDto>>>> GetList(
        [FromQuery] string? status, [FromQuery] int? agencyId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var filter = new MergerFilter { Status = status, AgencyId = agencyId, Page = page, PageSize = pageSize };
        var (items, total) = await _uow.Mergers.GetListAsync(filter, ct);
        return Ok(ApiResponse<List<MergerDto>>.Success(items.Select(MapDto).ToList(),
            new PaginationMeta { Page = page, PageSize = pageSize, TotalCount = total }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<MergerDto>>> GetById(int id, CancellationToken ct)
    {
        var merger = await _uow.Mergers.GetByIdAsync(id, ct);
        if (merger == null) return NotFound(ApiResponse<MergerDto>.Failure("Merger not found"));
        return Ok(ApiResponse<MergerDto>.Success(MapDto(merger)));
    }

    [HttpGet("{id:int}/preview")]
    public async Task<ActionResult<ApiResponse<MergerPreviewDto>>> Preview(int id, CancellationToken ct)
    {
        var merger = await _uow.Mergers.GetByIdAsync(id, ct);
        if (merger == null) return NotFound(ApiResponse<MergerPreviewDto>.Failure("Merger not found"));

        var survivingAgency = await _uow.Agencies.GetByIdAsync(merger.SurvivingAgencyId, ct);
        var preview = new MergerPreviewDto
        {
            MergerId = merger.Id,
            SurvivingAgencyId = merger.SurvivingAgencyId,
            SurvivingAgencyName = survivingAgency?.AgencyName
        };

        // Get surviving agency's producer NPNs for conflict detection
        var survivingNPNs = await _db.Personnel
            .Where(p => p.AgencyId == merger.SurvivingAgencyId && p.PersonnelType == PersonnelType.Producer)
            .Join(_db.Producers, p => p.Id, pr => pr.PersonnelId, (p, pr) => pr.NPN)
            .Where(npn => npn != null)
            .ToListAsync(ct);

        foreach (var participant in merger.Participants)
        {
            var absorbedPersonnel = await _db.Personnel
                .Where(p => p.AgencyId == participant.AbsorbedAgencyId && !p.IsDeleted)
                .CountAsync(ct);

            var absorbedProducers = await _db.Personnel
                .Where(p => p.AgencyId == participant.AbsorbedAgencyId && p.PersonnelType == PersonnelType.Producer && !p.IsDeleted)
                .ToListAsync(ct);

            var producerIds = absorbedProducers.Select(p => p.Id).ToList();

            var producers = await _db.Producers
                .Where(pr => producerIds.Contains(pr.PersonnelId) && !pr.IsDeleted)
                .ToListAsync(ct);

            var licenseCount = await _db.Licenses.Where(l => producers.Select(p => p.Id).Contains(l.ProducerId)).CountAsync(ct);
            var apptCount = await _db.CarrierAppointments.Where(a => producers.Select(p => p.Id).Contains(a.ProducerId)).CountAsync(ct);

            var duplicateNPNs = producers
                .Where(pr => pr.NPN != null && survivingNPNs.Contains(pr.NPN))
                .Select(pr => pr.NPN!)
                .ToList();

            preview.AbsorbedAgencies.Add(new AbsorbedAgencyPreviewDto
            {
                AgencyId = participant.AbsorbedAgencyId,
                AgencyName = participant.AbsorbedAgency.AgencyName,
                PersonnelCount = absorbedPersonnel,
                ProducerCount = absorbedProducers.Count,
                LicenseCount = licenseCount,
                AppointmentCount = apptCount,
                DuplicateNPNs = duplicateNPNs
            });

            if (duplicateNPNs.Count > 0)
                preview.Conflicts.Add($"Agency {participant.AbsorbedAgency.AgencyName} has {duplicateNPNs.Count} duplicate NPN(s): {string.Join(", ", duplicateNPNs)}");
        }

        preview.TotalPersonnelToTransfer = preview.AbsorbedAgencies.Sum(a => a.PersonnelCount);

        // Update merger status to Previewed
        merger.Status = MergerStatus.Previewed;
        _uow.Mergers.Update(merger);
        await _uow.SaveChangesAsync(ct);

        return Ok(ApiResponse<MergerPreviewDto>.Success(preview));
    }

    [HttpGet("{id:int}/lineage")]
    public async Task<ActionResult<ApiResponse<List<EntityLineageDto>>>> GetLineage(int id, CancellationToken ct)
    {
        var items = await _uow.Mergers.GetLineageAsync(id, ct);
        return Ok(ApiResponse<List<EntityLineageDto>>.Success(items.Select(MapLineage).ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MergerDto>>> Create([FromBody] CreateMergerDto dto, CancellationToken ct)
    {
        var survivingAgency = await _uow.Agencies.GetByIdAsync(dto.SurvivingAgencyId, ct);
        if (survivingAgency == null) return BadRequest(ApiResponse<MergerDto>.Failure("Surviving agency not found"));

        var merger = new Merger
        {
            SurvivingAgencyId = dto.SurvivingAgencyId,
            Status = MergerStatus.Draft,
            InitiatedBy = "system",
            InitiatedAt = DateTime.UtcNow,
            Notes = dto.Notes,
            CreatedBy = "system"
        };

        foreach (var absorbedId in dto.AbsorbedAgencyIds)
        {
            var absorbedAgency = await _uow.Agencies.GetByIdAsync(absorbedId, ct);
            if (absorbedAgency == null) return BadRequest(ApiResponse<MergerDto>.Failure($"Absorbed agency {absorbedId} not found"));
            merger.Participants.Add(new MergerParticipant
            {
                AbsorbedAgencyId = absorbedId, PersonnelTransferred = 0, CreatedBy = "system"
            });
        }

        _uow.Mergers.Add(merger);
        await _uow.SaveChangesAsync(ct);

        var created = await _uow.Mergers.GetByIdAsync(merger.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = merger.Id }, ApiResponse<MergerDto>.Success(MapDto(created!)));
    }

    [HttpPost("{id:int}/execute")]
    public async Task<ActionResult<ApiResponse<MergerDto>>> Execute(int id, CancellationToken ct)
    {
        var merger = await _uow.Mergers.GetByIdAsync(id, ct);
        if (merger == null) return NotFound(ApiResponse<MergerDto>.Failure("Merger not found"));
        if (merger.Status == MergerStatus.Completed)
            return BadRequest(ApiResponse<MergerDto>.Failure("Merger already completed"));
        if (merger.Status == MergerStatus.Cancelled)
            return BadRequest(ApiResponse<MergerDto>.Failure("Merger is cancelled"));

        await _uow.BeginTransactionAsync(ct);
        try
        {
            merger.Status = MergerStatus.Executing;
            var now = DateTime.UtcNow;

            foreach (var participant in merger.Participants)
            {
                // Transfer all personnel from absorbed agency to surviving agency
                var personnelToTransfer = await _db.Personnel
                    .Where(p => p.AgencyId == participant.AbsorbedAgencyId && !p.IsDeleted)
                    .ToListAsync(ct);

                foreach (var person in personnelToTransfer)
                {
                    person.AgencyId = merger.SurvivingAgencyId;
                    person.ModifiedBy = "merger";

                    _db.EntityLineages.Add(new EntityLineage
                    {
                        MergerId = merger.Id,
                        EntityType = "Personnel",
                        SourceEntityId = person.Id,
                        TargetEntityId = person.Id,
                        SourceAgencyId = participant.AbsorbedAgencyId,
                        TargetAgencyId = merger.SurvivingAgencyId,
                        Action = "Transferred",
                        RecordedAt = now,
                        RecordedBy = "system",
                        CreatedBy = "system"
                    });
                }

                // Mark absorbed agency as merged
                var absorbedAgency = await _db.Agencies.FindAsync(new object[] { participant.AbsorbedAgencyId }, ct);
                if (absorbedAgency != null)
                {
                    absorbedAgency.IsMerged = true;
                    absorbedAgency.MergedIntoId = merger.SurvivingAgencyId;
                    absorbedAgency.MergedAt = now;
                    absorbedAgency.IsActive = false;
                    absorbedAgency.ModifiedBy = "merger";

                    _db.EntityLineages.Add(new EntityLineage
                    {
                        MergerId = merger.Id,
                        EntityType = "Agency",
                        SourceEntityId = participant.AbsorbedAgencyId,
                        TargetEntityId = merger.SurvivingAgencyId,
                        SourceAgencyId = participant.AbsorbedAgencyId,
                        TargetAgencyId = merger.SurvivingAgencyId,
                        Action = "Merged",
                        RecordedAt = now,
                        RecordedBy = "system",
                        CreatedBy = "system"
                    });
                }

                participant.PersonnelTransferred = personnelToTransfer.Count;
            }

            merger.Status = MergerStatus.Completed;
            merger.ExecutedAt = now;
            merger.ExecutedBy = "system";
            merger.ModifiedBy = "system";

            await _db.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            var result = await _uow.Mergers.GetByIdAsync(id, ct);
            return Ok(ApiResponse<MergerDto>.Success(MapDto(result!)));
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ApiResponse<MergerDto>>> Cancel(int id, CancellationToken ct)
    {
        var merger = await _uow.Mergers.GetByIdAsync(id, ct);
        if (merger == null) return NotFound(ApiResponse<MergerDto>.Failure("Merger not found"));
        if (merger.Status != MergerStatus.Draft && merger.Status != MergerStatus.Previewed)
            return BadRequest(ApiResponse<MergerDto>.Failure("Can only cancel Draft or Previewed mergers"));

        merger.Status = MergerStatus.Cancelled;
        merger.ModifiedBy = "system";
        _uow.Mergers.Update(merger);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<MergerDto>.Success(MapDto(merger)));
    }

    private static MergerDto MapDto(Merger m) => new()
    {
        Id = m.Id, SurvivingAgencyId = m.SurvivingAgencyId,
        SurvivingAgencyName = m.SurvivingAgency?.AgencyName,
        Status = m.Status, InitiatedBy = m.InitiatedBy, InitiatedAt = m.InitiatedAt,
        ExecutedAt = m.ExecutedAt, ExecutedBy = m.ExecutedBy, Notes = m.Notes,
        Participants = m.Participants.Select(p => new MergerParticipantDto
        {
            Id = p.Id, AbsorbedAgencyId = p.AbsorbedAgencyId,
            AbsorbedAgencyName = p.AbsorbedAgency?.AgencyName,
            PersonnelTransferred = p.PersonnelTransferred
        }).ToList()
    };

    private static EntityLineageDto MapLineage(EntityLineage l) => new()
    {
        Id = l.Id, MergerId = l.MergerId, EntityType = l.EntityType,
        SourceEntityId = l.SourceEntityId, TargetEntityId = l.TargetEntityId,
        SourceAgencyId = l.SourceAgencyId, SourceAgencyName = l.SourceAgency?.AgencyName,
        TargetAgencyId = l.TargetAgencyId, TargetAgencyName = l.TargetAgency?.AgencyName,
        Action = l.Action, RecordedAt = l.RecordedAt
    };
}
