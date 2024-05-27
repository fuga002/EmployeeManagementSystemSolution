using System.Text.Json.Serialization;

namespace EMS.BaseLibrary.Entities;

public class SanctionType:BaseEntity
{
    
    //Many to one relationship with Vacation
    [JsonIgnore]
    public List<Sanction>? Sanctions { get; set; }
}