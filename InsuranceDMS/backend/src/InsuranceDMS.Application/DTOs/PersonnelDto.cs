using InsuranceDMS.Domain.Enums;

namespace InsuranceDMS.Application.DTOs;

public class PersonnelDto
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public string AgencyName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public PersonnelType PersonnelType { get; set; }
    public string? Title { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public bool IsActive { get; set; }
    public ProducerDto? Producer { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePersonnelDto
{
    public int AgencyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public PersonnelType PersonnelType { get; set; }
    public string? Title { get; set; }
    public DateTime? HireDate { get; set; }
    // Producer fields (only used when PersonnelType = Producer)
    public string? NPN { get; set; }
    public string? ResidentState { get; set; }
    public string? SSNLast4 { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? EOExpirationDate { get; set; }
}

public class UpdatePersonnelDto : CreatePersonnelDto
{
    public bool IsActive { get; set; }
    public DateTime? TerminationDate { get; set; }
}
