using System.ComponentModel.DataAnnotations;

namespace EMS.BaseLibrary.Entities;

public class Vacation:OtherBaseEntity
{
    [Required]
    public DateTime StartDate { get; set; }
    public int NumberOfDays { get; set; }
    public DateTime EndDate  => StartDate.AddDays(NumberOfDays);

    //Many-to-one relationship with vacation type
    public VacationType? VacationType { get; set; }
    public int VacationId { get; set; }
}