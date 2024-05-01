namespace EMS.BaseLibrary.Entities;

public class Branch:BaseEntity
{
    //Many to one relationship with Department
    public Department? Department { get; set; }
    public int DepartmentId { get; set; }
    
    //Relationship : One to many with Employee
    public List<Employee>? Employees { get; set; }
}