using System.Text.Json.Serialization;

namespace EMS.BaseLibrary.Entities;

public class GeneralDepartment:BaseEntity
{
    //One to many relationship with Department
    [JsonIgnore]
    public List<Department>? Departments { get; set; }
    
}